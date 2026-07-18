using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.PurchaseOrder;

using System.Data;
using System.Linq;

namespace JuJuBiAPI.Services
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly IDbConnection _connection;

        public PurchaseOrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<PurchaseOrder> CreateAsync(CreatePurchaseOrderDto dto)
        {
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
                        "No warehouse (HeadOffice outlet) is configured. Set an outlet's HeadOffice flag before creating purchase orders.");

                var insertHeaderSql = @"
                    INSERT INTO PurchaseOrders
                    (
                        SupplierID,
                        OutletID,
                        OrderDate,
                        ExpectedDate,
                        SubTotal,
                        DiscountAmount,
                        TaxAmount,
                        GrandTotal,
                        Status,
                        Note,
                        CreatedAt
                    )
                    VALUES
                    (
                        @SupplierID,
                        @OutletID,
                        GETDATE(),
                        @ExpectedDate,
                        0, 0, 0, 0,
                        'Draft',
                        @Note,
                        GETDATE()
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

                var purchaseOrderId = await _connection.ExecuteScalarAsync<int>(
                    insertHeaderSql,
                    new { dto.SupplierID, OutletID = warehouseOutletId.Value, dto.ExpectedDate, dto.Note },
                    transaction);

                decimal subTotal = 0, discountAmount = 0, taxAmount = 0, grandTotal = 0;

                var insertItemSql = @"
                    INSERT INTO PurchaseOrderItems
                    (
                        PurchaseOrderID,
                        ProNumY,
                        Quantity,
                        UnitCost,
                        DiscountPercent,
                        DiscountAmount,
                        TaxPercent,
                        TaxAmount,
                        SubTotal,
                        TotalCost,
                        ReceivedQty,
                        CreatedAt
                    )
                    VALUES
                    (
                        @PurchaseOrderID,
                        @ProNumY,
                        @Quantity,
                        @UnitCost,
                        @DiscountPercent,
                        @DiscountAmount,
                        @TaxPercent,
                        @TaxAmount,
                        @SubTotal,
                        @TotalCost,
                        0,
                        GETDATE()
                    );
                ";

                foreach (var itemDto in dto.Items)
                {
                    var item = CalculateItem(itemDto);

                    await _connection.ExecuteAsync(insertItemSql, new
                    {
                        PurchaseOrderID = purchaseOrderId,
                        item.ProNumY,
                        item.Quantity,
                        item.UnitCost,
                        item.DiscountPercent,
                        item.DiscountAmount,
                        item.TaxPercent,
                        item.TaxAmount,
                        item.SubTotal,
                        item.TotalCost
                    }, transaction);

                    subTotal += item.SubTotal;
                    discountAmount += item.DiscountAmount;
                    taxAmount += item.TaxAmount;
                    grandTotal += item.TotalCost;
                }

                var purchaseOrderNo = $"PO{purchaseOrderId:D6}";

                var updateHeaderSql = @"
                    UPDATE PurchaseOrders
                    SET
                        PurchaseOrderNo = @PurchaseOrderNo,
                        SubTotal = @SubTotal,
                        DiscountAmount = @DiscountAmount,
                        TaxAmount = @TaxAmount,
                        GrandTotal = @GrandTotal,
                        Status = 'Ordered'
                    WHERE PurchaseOrderID = @PurchaseOrderID;
                ";

                await _connection.ExecuteAsync(updateHeaderSql, new
                {
                    PurchaseOrderNo = purchaseOrderNo,
                    SubTotal = subTotal,
                    DiscountAmount = discountAmount,
                    TaxAmount = taxAmount,
                    GrandTotal = grandTotal,
                    PurchaseOrderID = purchaseOrderId
                }, transaction);

                transaction.Commit();

                return await GetByIdAsync(purchaseOrderId)
                    ?? throw new InvalidOperationException("Failed to load created purchase order.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int purchaseOrderId)
        {
            var sql = @"
                SELECT * FROM PurchaseOrders
                WHERE PurchaseOrderID = @PurchaseOrderID;

                SELECT * FROM PurchaseOrderItems
                WHERE PurchaseOrderID = @PurchaseOrderID;
            ";

            using var multi = await _connection.QueryMultipleAsync(
                sql,
                new { PurchaseOrderID = purchaseOrderId });

            var purchaseOrder = await multi.ReadSingleOrDefaultAsync<PurchaseOrder>();

            if (purchaseOrder == null)
                return null;

            purchaseOrder.PurchaseOrderItems =
                (await multi.ReadAsync<PurchaseOrderItem>()).ToList();

            return purchaseOrder;
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            var sql = @"
                SELECT * FROM PurchaseOrders
                ORDER BY CreatedAt DESC;
            ";

            var purchaseOrders = (await _connection.QueryAsync<PurchaseOrder>(sql)).ToList();

            if (!purchaseOrders.Any())
                return purchaseOrders;          

            var ids = purchaseOrders.Select(p => p.PurchaseOrderID).ToArray();

            var itemsSql = @"
                SELECT * FROM PurchaseOrderItems
                WHERE PurchaseOrderID IN @Ids;
            ";

            var items = (await _connection.QueryAsync<PurchaseOrderItem>(itemsSql, new { Ids = ids })).ToList();

            var itemsByOrder = items.GroupBy(i => i.PurchaseOrderID)
                                    .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var po in purchaseOrders)
            {
                itemsByOrder.TryGetValue(po.PurchaseOrderID, out var poItems);
                po.PurchaseOrderItems = poItems ?? new List<PurchaseOrderItem>();
            }

            return purchaseOrders;
        }

        public async Task<List<PurchaseOrder>> GetBySupplierAsync(int supplierId)
        {
            var sql = @"
                SELECT * FROM PurchaseOrders
                WHERE SupplierID = @SupplierID
                ORDER BY CreatedAt DESC;
            ";

            var purchaseOrders = await _connection.QueryAsync<PurchaseOrder>(
                sql, new { SupplierID = supplierId });

            return purchaseOrders.ToList();
        }

        public async Task<List<PurchaseOrder>> GetByOutletAsync(int outletId)
        {
            var sql = @"
                SELECT * FROM PurchaseOrders
                WHERE OutletID = @OutletID
                ORDER BY CreatedAt DESC;
            ";

            var purchaseOrders = await _connection.QueryAsync<PurchaseOrder>(
                sql, new { OutletID = outletId });

            return purchaseOrders.ToList();
        }

        public async Task<bool> UpdateStatusAsync(int purchaseOrderId, string status)
        {
            var sql = @"
                UPDATE PurchaseOrders
                SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE PurchaseOrderID = @PurchaseOrderID;
            ";

            var rows = await _connection.ExecuteAsync(sql, new
            {
                PurchaseOrderID = purchaseOrderId,
                Status = status
            });

            return rows > 0;
        }

        public async Task<bool> ReceiveAsync(int purchaseOrderId, ReceivePurchaseOrderDto dto)
        {
            var purchaseOrder = await GetByIdAsync(purchaseOrderId);

            if (purchaseOrder == null)
                return false;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                foreach (var receiveItem in dto.Items)
                {
                    var item = purchaseOrder.PurchaseOrderItems
                        .FirstOrDefault(i => i.PurchaseOrderItemID == receiveItem.PurchaseOrderItemID);

                    if (item == null)
                        throw new InvalidOperationException(
                            $"Purchase order item {receiveItem.PurchaseOrderItemID} does not belong to this purchase order.");

                    var remaining = item.Quantity - item.ReceivedQty;

                    if (receiveItem.ReceivedQty > remaining)
                        throw new InvalidOperationException(
                            $"Cannot receive {receiveItem.ReceivedQty} for product {item.ProNumY}; only {remaining} remaining.");

                    await _connection.ExecuteAsync(@"
                        UPDATE PurchaseOrderItems
                        SET ReceivedQty = ReceivedQty + @ReceivedQty
                        WHERE PurchaseOrderItemID = @PurchaseOrderItemID;
                    ", new
                    {
                        receiveItem.ReceivedQty,
                        receiveItem.PurchaseOrderItemID
                    }, transaction);

                    // ProNumY is the product's real primary key; ProductStocks keys off
                    // the surrogate ProID, so resolve it before touching stock.
                    var proId = await _connection.QueryFirstOrDefaultAsync<int?>(@"
                        SELECT ProID FROM TPRProducts WHERE ProNumY = @ProNumY;
                    ", new { item.ProNumY }, transaction);

                    if (proId == null)
                        throw new InvalidOperationException(
                            $"Product {item.ProNumY} was not found.");

                    var existingStock = await _connection.QueryFirstOrDefaultAsync<int?>(@"
                        SELECT StockID FROM ProductStocks
                        WHERE ProductID = @ProductID AND OutletId = @OutletId;
                    ", new { ProductID = proId.Value, OutletId = purchaseOrder.OutletID }, transaction);

                    if (existingStock.HasValue)
                    {
                        await _connection.ExecuteAsync(@"
                            UPDATE ProductStocks
                            SET
                                StockQty = StockQty + @ReceivedQty,
                                LastUpdated = GETDATE()
                            WHERE StockID = @StockID;
                        ", new { receiveItem.ReceivedQty, StockID = existingStock.Value }, transaction);
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
                            OutletId = purchaseOrder.OutletID,
                            StockQty = receiveItem.ReceivedQty
                        }, transaction);
                    }

                    item.ReceivedQty += receiveItem.ReceivedQty;
                }

                var newStatus = purchaseOrder.PurchaseOrderItems.All(i => i.ReceivedQty >= i.Quantity)
                    ? "Received"
                    : "PartiallyReceived";

                await _connection.ExecuteAsync(@"
                    UPDATE PurchaseOrders
                    SET Status = @Status, UpdatedAt = GETDATE()
                    WHERE PurchaseOrderID = @PurchaseOrderID;
                ", new { Status = newStatus, PurchaseOrderID = purchaseOrderId }, transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int purchaseOrderId)
        {
            var sql = @"
                DELETE FROM PurchaseOrderItems WHERE PurchaseOrderID = @PurchaseOrderID;
                DELETE FROM PurchaseOrders WHERE PurchaseOrderID = @PurchaseOrderID;
            ";

            var rows = await _connection.ExecuteAsync(sql, new { PurchaseOrderID = purchaseOrderId });

            return rows > 0;
        }

        private static PurchaseOrderItem CalculateItem(CreatePurchaseOrderItemDto dto)
        {
            var subTotal = dto.Quantity * dto.UnitCost;
            var discountAmount = subTotal * dto.DiscountPercent / 100;
            var afterDiscount = subTotal - discountAmount;
            var taxAmount = afterDiscount * dto.TaxPercent / 100;
            var totalCost = afterDiscount + taxAmount;

            return new PurchaseOrderItem
            {
                ProNumY = dto.ProNumY,
                Quantity = dto.Quantity,
                UnitCost = dto.UnitCost,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = discountAmount,
                TaxPercent = dto.TaxPercent,
                TaxAmount = taxAmount,
                SubTotal = subTotal,
                TotalCost = totalCost
            };
        }
    }
}
