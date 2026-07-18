using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.PurchaseOrder;

namespace JuJuBiAPI.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder> CreateAsync(CreatePurchaseOrderDto dto);

        Task<PurchaseOrder?> GetByIdAsync(int purchaseOrderId);

        Task<List<PurchaseOrder>> GetAllAsync();

        Task<List<PurchaseOrder>> GetBySupplierAsync(int supplierId);

        Task<List<PurchaseOrder>> GetByOutletAsync(int outletId);

        Task<bool> UpdateStatusAsync(int purchaseOrderId, string status);

        Task<bool> ReceiveAsync(int purchaseOrderId, ReceivePurchaseOrderDto dto);

        Task<bool> DeleteAsync(int purchaseOrderId);
    }
}
