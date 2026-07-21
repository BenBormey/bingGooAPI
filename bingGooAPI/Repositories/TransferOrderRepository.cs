using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.TransferOrder;
using JuJuBiAPI.Queries;

using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class TransferOrderRepository : ITransferOrderRepository
    {
        private readonly IDbConnection _connection;

        public TransferOrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<TransferOrder> CreateAsync(CreateTransferOrderDto dto)
        {
            if (dto.FromOutletId == dto.ToOutletId)
                throw new InvalidOperationException(
                    "Source and destination outlets must be different.");

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                var fromExists = await _connection.ExecuteScalarAsync<int>(
                    TransferOrderQueries.OutletExists, new { OutletId = dto.FromOutletId }, transaction);

                var toExists = await _connection.ExecuteScalarAsync<int>(
                    TransferOrderQueries.OutletExists, new { OutletId = dto.ToOutletId }, transaction);

                if (fromExists == 0 || toExists == 0)
                    throw new InvalidOperationException("Source or destination outlet does not exist.");

                var transferOrderId = await _connection.ExecuteScalarAsync<int>(
                    TransferOrderQueries.InsertHeader,
                    new { dto.FromOutletId, dto.ToOutletId, dto.Remark, dto.CreatedBy },
                    transaction);

                foreach (var item in dto.Items)
                {
                    await _connection.ExecuteAsync(TransferOrderQueries.InsertItem, new
                    {
                        TransferOrderId = transferOrderId,
                        item.ProNumY,
                        item.Qty,
                        item.UnitCost,
                        item.Remark
                    }, transaction);
                }

                var transferNo = $"TR{transferOrderId:D6}";

                await _connection.ExecuteAsync(
                    TransferOrderQueries.SetTransferNo,
                    new { TransferNo = transferNo, TransferOrderId = transferOrderId },
                    transaction);

                transaction.Commit();

                return await GetByIdAsync(transferOrderId)
                    ?? throw new InvalidOperationException("Failed to load created transfer order.");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<TransferOrder?> GetByIdAsync(int transferOrderId)
        {
            using var multi = await _connection.QueryMultipleAsync(
                TransferOrderQueries.GetById,
                new { TransferOrderId = transferOrderId });

            var transferOrder = await multi.ReadSingleOrDefaultAsync<TransferOrder>();

            if (transferOrder == null)
                return null;

            transferOrder.TransferOrderItems =
                (await multi.ReadAsync<TransferOrderItem>()).ToList();

            return transferOrder;
        }

        public async Task<List<TransferOrder>> GetAllAsync()
        {
            var transferOrders = await _connection.QueryAsync<TransferOrder>(TransferOrderQueries.GetAll);

            return transferOrders.ToList();
        }

        public async Task<List<TransferOrder>> GetByOutletAsync(int outletId)
        {
            var transferOrders = await _connection.QueryAsync<TransferOrder>(
                TransferOrderQueries.GetByOutlet, new { OutletId = outletId });

            return transferOrders.ToList();
        }

        public async Task<bool> UpdateStatusAsync(int transferOrderId, string status)
        {
            var rows = await _connection.ExecuteAsync(TransferOrderQueries.UpdateStatus, new
            {
                TransferOrderId = transferOrderId,
                Status = status
            });

            return rows > 0;
        }

        // Ships the transfer: deducts every line's Qty from the source (From)
        // outlet's stock, then marks the order Approved. Stock is now "in transit"
        // until the destination receives it.
        public async Task<bool> ApproveAsync(int transferOrderId, ApproveTransferOrderDto dto)
        {
            var transferOrder = await GetByIdAsync(transferOrderId);

            if (transferOrder == null)
                return false;

            if (transferOrder.Status != "Pending")
                throw new InvalidOperationException(
                    $"Only a Pending transfer can be approved (current status: {transferOrder.Status}).");

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                foreach (var item in transferOrder.TransferOrderItems)
                {
                    var sourceQty = await _connection.QueryFirstOrDefaultAsync<decimal?>(
                        TransferOrderQueries.GetSourceStock,
                        new { item.ProNumY, OutletId = transferOrder.FromOutletId }, transaction);

                    if (sourceQty == null || sourceQty < item.Qty)
                        throw new InvalidOperationException(
                            $"Source outlet does not have enough stock of {item.ProNumY} to transfer {item.Qty}.");

                    await _connection.ExecuteAsync(
                        TransferOrderQueries.DeductSourceStock,
                        new { item.Qty, item.ProNumY, OutletId = transferOrder.FromOutletId }, transaction);
                }

                await _connection.ExecuteAsync(
                    TransferOrderQueries.SetApproved,
                    new { ApprovedBy = dto.ApprovedBy, TransferOrderId = transferOrderId }, transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Receives the transfer: adds the received quantities to the destination
        // (To) outlet's stock and marks the order Received or PartiallyReceived.
        public async Task<bool> ReceiveAsync(int transferOrderId, ReceiveTransferOrderDto dto)
        {
            var transferOrder = await GetByIdAsync(transferOrderId);

            if (transferOrder == null)
                return false;

            if (transferOrder.Status != "Approved" && transferOrder.Status != "PartiallyReceived")
                throw new InvalidOperationException(
                    $"Only an Approved transfer can be received (current status: {transferOrder.Status}).");

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                foreach (var receiveItem in dto.Items)
                {
                    var item = transferOrder.TransferOrderItems
                        .FirstOrDefault(i => i.TransferOrderItemId == receiveItem.TransferOrderItemId);

                    if (item == null)
                        throw new InvalidOperationException(
                            $"Transfer item {receiveItem.TransferOrderItemId} does not belong to this transfer order.");

                    var remaining = item.Qty - item.ReceivedQty;

                    if (receiveItem.ReceivedQty > remaining)
                        throw new InvalidOperationException(
                            $"Cannot receive {receiveItem.ReceivedQty} for product {item.ProNumY}; only {remaining} remaining.");

                    var destHasRow = await _connection.QueryFirstOrDefaultAsync<bool>(
                        TransferOrderQueries.DestHasStockRow,
                        new { item.ProNumY, OutletId = transferOrder.ToOutletId }, transaction);

                    if (destHasRow)
                    {
                        await _connection.ExecuteAsync(
                            TransferOrderQueries.AddDestStock,
                            new { Qty = receiveItem.ReceivedQty, item.ProNumY, OutletId = transferOrder.ToOutletId }, transaction);
                    }
                    else
                    {
                        await _connection.ExecuteAsync(
                            TransferOrderQueries.InsertDestStock,
                            new
                            {
                                OutletId = transferOrder.ToOutletId,
                                item.ProNumY,
                                StockQty = receiveItem.ReceivedQty
                            }, transaction);
                    }

                    await _connection.ExecuteAsync(
                        TransferOrderQueries.IncrementReceivedQty,
                        new { receiveItem.ReceivedQty, receiveItem.TransferOrderItemId }, transaction);

                    item.ReceivedQty += receiveItem.ReceivedQty;
                }

                var newStatus = transferOrder.TransferOrderItems.All(i => i.ReceivedQty >= i.Qty)
                    ? "Received"
                    : "PartiallyReceived";

                await _connection.ExecuteAsync(
                    TransferOrderQueries.SetReceived,
                    new { Status = newStatus, ReceivedBy = dto.ReceivedBy, TransferOrderId = transferOrderId }, transaction);

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int transferOrderId)
        {
            var rows = await _connection.ExecuteAsync(TransferOrderQueries.Delete, new { TransferOrderId = transferOrderId });

            return rows > 0;
        }
    }
}
