using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.OutletOrder;

using System.Data;

namespace JuJuBiAPI.Services
{
    public class OutletOrderRepository : IOutletOrderRepository
    {
        private readonly IDbConnection _connection;

        public OutletOrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<OutletOrder> CreateAsync(CreateOutletOrderDto dto)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                var isWarehouse = await _connection.QueryFirstOrDefaultAsync<bool>(
                    "SELECT HeadOffice FROM [dbo].[Outlet] WHERE Id = @OutletID;",
                    new { dto.OutletID },
                    transaction);

                if (isWarehouse)
                    throw new InvalidOperationException(
                        "The warehouse cannot place an outlet order against itself.");

                var insertHeaderSql = @"
                    INSERT INTO OutletOrders
                    (
                        OutletID,
                        OrderDate,
                        ExpectedDate,
                        Status,
                        Note,
                        CreatedAt
                    )
                    VALUES
                    (
                        @OutletID,
                        GETDATE(),
                        @ExpectedDate,
                        'Requested',
                        @Note,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

                var outletOrderId = await _connection.ExecuteScalarAsync<int>(
                    insertHeaderSql,
                    new { dto.OutletID, dto.ExpectedDate, dto.Note },
                    transaction);

                var insertItemSql = @"
                    INSERT INTO OutletOrderItems
                    (
                        OutletOrderID,
                        ProNumY,
                        RequestedQty,
                        FulfilledQty,
                        CreatedAt
                    )
                    VALUES
                    (
                        @OutletOrderID,
                        @ProNumY,
                        @RequestedQty,
                        0,
                        GETDATE()
                    );
                ";

                foreach (var item in dto.Items)
                {
                    await _connection.ExecuteAsync(insertItemSql, new
                    {
                        OutletOrderID = outletOrderId,
                        item.ProNumY,
                        item.RequestedQty
                    }, transaction);
                }

                var outletOrderNo = $"OO{outletOrderId:D6}";

                await _connection.ExecuteAsync(@"
                    UPDATE OutletOrders
                    SET OutletOrderNo = @OutletOrderNo
                    WHERE OutletOrderID = @OutletOrderID;
                ", new { OutletOrderNo = outletOrderNo, OutletOrderID = outletOrderId }, transaction);

                transaction.Commit();

                return await GetByIdAsync(outletOrderId)
                    ?? throw new InvalidOperationException("Failed to load created outlet order.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<OutletOrder?> GetByIdAsync(int outletOrderId)
        {
            var sql = @"
                SELECT * FROM OutletOrders
                WHERE OutletOrderID = @OutletOrderID;

                SELECT * FROM OutletOrderItems
                WHERE OutletOrderID = @OutletOrderID;
            ";

            using var multi = await _connection.QueryMultipleAsync(
                sql,
                new { OutletOrderID = outletOrderId });

            var outletOrder = await multi.ReadSingleOrDefaultAsync<OutletOrder>();

            if (outletOrder == null)
                return null;

            outletOrder.OutletOrderItems =
                (await multi.ReadAsync<OutletOrderItem>()).ToList();

            return outletOrder;
        }

        public async Task<List<OutletOrder>> GetAllAsync()
        {
            var sql = @"
                SELECT * FROM OutletOrders
                ORDER BY CreatedAt DESC;
            ";

            var outletOrders = await _connection.QueryAsync<OutletOrder>(sql);

            return outletOrders.ToList();
        }

        public async Task<List<OutletOrder>> GetByOutletAsync(int outletId)
        {
            var sql = @"
                SELECT * FROM OutletOrders
                WHERE OutletID = @OutletID
                ORDER BY CreatedAt DESC;
            ";

            var outletOrders = await _connection.QueryAsync<OutletOrder>(
                sql, new { OutletID = outletId });

            return outletOrders.ToList();
        }

        public async Task<bool> UpdateStatusAsync(int outletOrderId, string status)
        {
            var sql = @"
                UPDATE OutletOrders
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE OutletOrderID = @OutletOrderID;
            ";

            var rows = await _connection.ExecuteAsync(sql, new
            {
                OutletOrderID = outletOrderId,
                Status = status
            });

            return rows > 0;
        }

        public async Task<bool> FulfillAsync(int outletOrderId, FulfillOutletOrderDto dto)
        {
            var outletOrder = await GetByIdAsync(outletOrderId);

            if (outletOrder == null)
                return false;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                var warehouseOutletId = await _connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT TOP 1 Id FROM [dbo].[Outlet] WHERE HeadOffice = 1;",
                    transaction: transaction);

                if (warehouseOutletId == null)
                    throw new InvalidOperationException(
                        "No warehouse (HeadOffice outlet) is configured.");

                foreach (var fulfillItem in dto.Items)
                {
                    var item = outletOrder.OutletOrderItems
                        .FirstOrDefault(i => i.OutletOrderItemID == fulfillItem.OutletOrderItemID);

                    if (item == null)
                        throw new InvalidOperationException(
                            $"Outlet order item {fulfillItem.OutletOrderItemID} does not belong to this outlet order.");

                    var remaining = item.RequestedQty - item.FulfilledQty;

                    if (fulfillItem.FulfilledQty > remaining)
                        throw new InvalidOperationException(
                            $"Cannot fulfill {fulfillItem.FulfilledQty} for product {item.ProNumY}; only {remaining} remaining.");

                    var proId = await _connection.QueryFirstOrDefaultAsync<int?>(@"
                        SELECT ProID FROM TPRProducts WHERE ProNumY = @ProNumY;
                    ", new { item.ProNumY }, transaction);

                    if (proId == null)
                        throw new InvalidOperationException(
                            $"Product {item.ProNumY} was not found.");

                    var warehouseStock = await _connection.QueryFirstOrDefaultAsync<StockRow>(@"
                        SELECT StockID, StockQty FROM ProductStocks
                        WHERE ProductID = @ProductID AND OutletId = @OutletId;
                    ", new { ProductID = proId.Value, OutletId = warehouseOutletId.Value }, transaction);

                    if (warehouseStock == null || warehouseStock.StockQty < fulfillItem.FulfilledQty)
                        throw new InvalidOperationException(
                            $"Warehouse does not have enough stock of {item.ProNumY} to fulfill {fulfillItem.FulfilledQty}.");

                    await _connection.ExecuteAsync(@"
                        UPDATE ProductStocks
                        SET StockQty = StockQty - @FulfilledQty, LastUpdated = GETDATE()
                        WHERE StockID = @StockID;
                    ", new { fulfillItem.FulfilledQty, warehouseStock.StockID }, transaction);

                    var outletStock = await _connection.QueryFirstOrDefaultAsync<int?>(@"
                        SELECT StockID FROM ProductStocks
                        WHERE ProductID = @ProductID AND OutletId = @OutletId;
                    ", new { ProductID = proId.Value, OutletId = outletOrder.OutletID }, transaction);

                    if (outletStock.HasValue)
                    {
                        await _connection.ExecuteAsync(@"
                            UPDATE ProductStocks
                            SET StockQty = StockQty + @FulfilledQty, LastUpdated = GETDATE()
                            WHERE StockID = @StockID;
                        ", new { fulfillItem.FulfilledQty, StockID = outletStock.Value }, transaction);
                    }
                    else
                    {
                        await _connection.ExecuteAsync(@"
                            INSERT INTO ProductStocks
                                (ProductID, OutletId, StockQty, LastUpdated)
                            VALUES
                                (@ProductID, @OutletId, @StockQty, GETDATE());
                        ", new
                        {
                            ProductID = proId.Value,
                            OutletId = outletOrder.OutletID,
                            StockQty = fulfillItem.FulfilledQty
                        }, transaction);
                    }

                    await _connection.ExecuteAsync(@"
                        UPDATE OutletOrderItems
                        SET FulfilledQty = FulfilledQty + @FulfilledQty
                        WHERE OutletOrderItemID = @OutletOrderItemID;
                    ", new { fulfillItem.FulfilledQty, fulfillItem.OutletOrderItemID }, transaction);

                    item.FulfilledQty += fulfillItem.FulfilledQty;
                }

                var newStatus = outletOrder.OutletOrderItems.All(i => i.FulfilledQty >= i.RequestedQty)
                    ? "Fulfilled"
                    : "PartiallyFulfilled";

                await _connection.ExecuteAsync(@"
                    UPDATE OutletOrders
                    SET Status = @Status, UpdatedAt = GETDATE()
                    WHERE OutletOrderID = @OutletOrderID;
                ", new { Status = newStatus, OutletOrderID = outletOrderId }, transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int outletOrderId)
        {
            var sql = @"
                DELETE FROM OutletOrderItems WHERE OutletOrderID = @OutletOrderID;
                DELETE FROM OutletOrders WHERE OutletOrderID = @OutletOrderID;
            ";

            var rows = await _connection.ExecuteAsync(sql, new { OutletOrderID = outletOrderId });

            return rows > 0;
        }

        private class StockRow
        {
            public int StockID { get; set; }
            public int StockQty { get; set; }
        }
    }
}
