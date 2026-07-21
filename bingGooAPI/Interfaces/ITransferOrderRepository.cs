using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.TransferOrder;

namespace JuJuBiAPI.Interfaces
{
    public interface ITransferOrderRepository
    {
        Task<TransferOrder> CreateAsync(CreateTransferOrderDto dto);

        Task<TransferOrder?> GetByIdAsync(int transferOrderId);

        Task<List<TransferOrder>> GetAllAsync();

        Task<List<TransferOrder>> GetByOutletAsync(int outletId);

        Task<bool> UpdateStatusAsync(int transferOrderId, string status);

        // Ships the transfer: deducts stock from the source (From) outlet.
        Task<bool> ApproveAsync(int transferOrderId, ApproveTransferOrderDto dto);

        // Receives the transfer: adds stock to the destination (To) outlet.
        Task<bool> ReceiveAsync(int transferOrderId, ReceiveTransferOrderDto dto);

        Task<bool> DeleteAsync(int transferOrderId);
    }
}
