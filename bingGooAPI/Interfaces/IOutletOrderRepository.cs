using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.OutletOrder;

namespace JuJuBiAPI.Interfaces
{
    public interface IOutletOrderRepository
    {
        Task<OutletOrder> CreateAsync(CreateOutletOrderDto dto);

        Task<OutletOrder?> GetByIdAsync(int outletOrderId);

        Task<List<OutletOrder>> GetAllAsync();

        Task<List<OutletOrder>> GetByOutletAsync(int outletId);

        Task<bool> UpdateStatusAsync(int outletOrderId, string status);

        Task<bool> FulfillAsync(int outletOrderId, FulfillOutletOrderDto dto);

        Task<bool> DeleteAsync(int outletOrderId);

        Task<List<WarehouseStockDto>> GetWarehouseStockAsync();
    }
}
