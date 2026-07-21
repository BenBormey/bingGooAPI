using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.OutletOrder;
using JuJuBiAPI.Queries;

using System.Data;

namespace JuJuBiAPI.Repositories
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
                    OutletOrderQueries.IsWarehouse,
                    new { dto.OutletID },
                    transaction);

                if (isWarehouse)
                    throw new InvalidOperationException(
                        "The warehouse cannot place an outlet order against itself.");

                var outletOrderId = await _connection.ExecuteScalarAsync<int>(
                    OutletOrderQueries.InsertHeader,
                    new { dto.OutletID, dto.ExpectedDate, dto.Note },
                    transaction);

                foreach (var item in dto.Items)
                {
                    await _connection.ExecuteAsync(OutletOrderQueries.InsertItem, new
                    {
                        OutletOrderID = outletOrderId,
                        item.ProNumY,
                        item.RequestedQty
                    }, transaction);
                }

                var outletOrderNo = $"OO{outletOrderId:D6}";

                await _connection.ExecuteAsync(
                    OutletOrderQueries.SetOrderNo,
                    new { OutletOrderNo = outletOrderNo, OutletOrderID = outletOrderId },
                    transaction);

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
            using var multi = await _connection.QueryMultipleAsync(
                OutletOrderQueries.GetById,
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
            var outletOrders = await _connection.QueryAsync<OutletOrder>(OutletOrderQueries.GetAll);

            return outletOrders.ToList();
        }

        public async Task<List<OutletOrder>> GetByOutletAsync(int outletId)
        {
            var outletOrders = await _connection.QueryAsync<OutletOrder>(
                OutletOrderQueries.GetByOutlet, new { OutletID = outletId });

            return outletOrders.ToList();
        }

        public async Task<bool> UpdateStatusAsync(int outletOrderId, string status)
        {
            var rows = await _connection.ExecuteAsync(OutletOrderQueries.UpdateStatus, new
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
                    OutletOrderQueries.GetWarehouseOutletId,
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

                    // OutletStock is keyed directly by ProNumY + OutletId — no
                    // TPRProducts/Products indirection needed, unlike the old
                    // ProductStocks table.
                    var warehouseQty = await _connection.QueryFirstOrDefaultAsync<decimal?>(
                        OutletOrderQueries.GetWarehouseStock,
                        new { item.ProNumY, OutletId = warehouseOutletId.Value }, transaction);

                    if (warehouseQty == null || warehouseQty < fulfillItem.FulfilledQty)
                        throw new InvalidOperationException(
                            $"Warehouse does not have enough stock of {item.ProNumY} to fulfill {fulfillItem.FulfilledQty}.");

                    await _connection.ExecuteAsync(
                        OutletOrderQueries.DeductWarehouseStock,
                        new { fulfillItem.FulfilledQty, item.ProNumY, OutletId = warehouseOutletId.Value }, transaction);

                    var outletHasStockRow = await _connection.QueryFirstOrDefaultAsync<bool>(
                        OutletOrderQueries.OutletHasStockRow,
                        new { item.ProNumY, OutletId = outletOrder.OutletID }, transaction);

                    if (outletHasStockRow)
                    {
                        await _connection.ExecuteAsync(
                            OutletOrderQueries.AddOutletStock,
                            new { fulfillItem.FulfilledQty, item.ProNumY, OutletId = outletOrder.OutletID }, transaction);
                    }
                    else
                    {
                        await _connection.ExecuteAsync(
                            OutletOrderQueries.InsertOutletStock,
                            new
                            {
                                OutletId = outletOrder.OutletID,
                                item.ProNumY,
                                StockQty = fulfillItem.FulfilledQty
                            }, transaction);
                    }

                    await _connection.ExecuteAsync(
                        OutletOrderQueries.IncrementFulfilledQty,
                        new { fulfillItem.FulfilledQty, fulfillItem.OutletOrderItemID }, transaction);

                    item.FulfilledQty += fulfillItem.FulfilledQty;
                }

                var newStatus = outletOrder.OutletOrderItems.All(i => i.FulfilledQty >= i.RequestedQty)
                    ? "Fulfilled"
                    : "PartiallyFulfilled";

                await _connection.ExecuteAsync(
                    OutletOrderQueries.SetStatus,
                    new { Status = newStatus, OutletOrderID = outletOrderId }, transaction);

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
            var rows = await _connection.ExecuteAsync(OutletOrderQueries.Delete, new { OutletOrderID = outletOrderId });

            return rows > 0;
        }
    }
}
