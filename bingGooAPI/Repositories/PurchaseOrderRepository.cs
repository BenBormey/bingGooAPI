using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.PurchaseOrder;
using JuJuBiAPI.Queries;

using System.Data;
using System.Linq;

namespace JuJuBiAPI.Repositories
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
                    PurchaseOrderQueries.GetWarehouseOutletId,
                    transaction: transaction);

                if (warehouseOutletId == null)
                    throw new InvalidOperationException(
                        "No warehouse (HeadOffice outlet) is configured. Set an outlet's HeadOffice flag before creating purchase orders.");

                var purchaseOrderId = await _connection.ExecuteScalarAsync<int>(
                    PurchaseOrderQueries.InsertHeader,
                    new { dto.SupplierID, OutletID = warehouseOutletId.Value, dto.ExpectedDate, dto.Note },
                    transaction);

                decimal subTotal = 0, discountAmount = 0, taxAmount = 0, grandTotal = 0;

                foreach (var itemDto in dto.Items)
                {
                    var item = CalculateItem(itemDto);

                    await _connection.ExecuteAsync(PurchaseOrderQueries.InsertItem, new
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

                await _connection.ExecuteAsync(PurchaseOrderQueries.UpdateHeaderTotals, new
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
            using var multi = await _connection.QueryMultipleAsync(
                PurchaseOrderQueries.GetById,
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
            var purchaseOrders = (await _connection.QueryAsync<PurchaseOrder>(PurchaseOrderQueries.GetAll)).ToList();

            if (!purchaseOrders.Any())
                return purchaseOrders;

            var ids = purchaseOrders.Select(p => p.PurchaseOrderID).ToArray();

            var items = (await _connection.QueryAsync<PurchaseOrderItem>(
                PurchaseOrderQueries.GetItemsByOrderIds, new { Ids = ids })).ToList();

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
            var purchaseOrders = await _connection.QueryAsync<PurchaseOrder>(
                PurchaseOrderQueries.GetBySupplier, new { SupplierID = supplierId });

            return purchaseOrders.ToList();
        }

        public async Task<List<PurchaseOrder>> GetByOutletAsync(int outletId)
        {
            var purchaseOrders = await _connection.QueryAsync<PurchaseOrder>(
                PurchaseOrderQueries.GetByOutlet, new { OutletID = outletId });

            return purchaseOrders.ToList();
        }

        public async Task<bool> UpdateStatusAsync(int purchaseOrderId, string status)
        {
            var rows = await _connection.ExecuteAsync(PurchaseOrderQueries.UpdateStatus, new
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

                    await _connection.ExecuteAsync(PurchaseOrderQueries.IncrementReceivedQty, new
                    {
                        receiveItem.ReceivedQty,
                        receiveItem.PurchaseOrderItemID
                    }, transaction);

                    // Guard: make sure the product exists in the master table.
                    var proId = await _connection.QueryFirstOrDefaultAsync<int?>(
                        PurchaseOrderQueries.GetProIdByProNumY,
                        new { item.ProNumY }, transaction);

                    if (proId == null)
                        throw new InvalidOperationException(
                            $"Product {item.ProNumY} was not found.");

                    // OutletStock is keyed by ProNumY + OutletId (no surrogate StockID).
                    var stockExists = await _connection.ExecuteScalarAsync<int>(
                        PurchaseOrderQueries.OutletStockExists,
                        new { item.ProNumY, OutletId = purchaseOrder.OutletID }, transaction) == 1;

                    if (stockExists)
                    {
                        await _connection.ExecuteAsync(
                            PurchaseOrderQueries.AddProductStock,
                            new { receiveItem.ReceivedQty, item.ProNumY, OutletId = purchaseOrder.OutletID }, transaction);
                    }
                    else
                    {
                        await _connection.ExecuteAsync(
                            PurchaseOrderQueries.InsertProductStock,
                            new
                            {
                                OutletId = purchaseOrder.OutletID,
                                item.ProNumY,
                                StockQty = receiveItem.ReceivedQty
                            }, transaction);
                    }

                    item.ReceivedQty += receiveItem.ReceivedQty;
                }

                var newStatus = purchaseOrder.PurchaseOrderItems.All(i => i.ReceivedQty >= i.Quantity)
                    ? "Received"
                    : "PartiallyReceived";

                await _connection.ExecuteAsync(
                    PurchaseOrderQueries.SetStatus,
                    new { Status = newStatus, PurchaseOrderID = purchaseOrderId }, transaction);

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
            var rows = await _connection.ExecuteAsync(PurchaseOrderQueries.Delete, new { PurchaseOrderID = purchaseOrderId });

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
