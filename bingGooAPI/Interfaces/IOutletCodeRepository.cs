using bingGooAPI.Entities;
using bingGooAPI.Models.Outlet;

namespace bingGooAPI.Interfaces
{
    public interface IOutletCodeRepository
    {
        Task<IEnumerable<OutletCodeEntity>> GetAllAsync();

        Task<OutletCodeEntity?> GetByIdAsync(int id);

        Task<OutletCodeEntity> AddAsync(CreateOutletCodeDto dto);

        Task<bool> UpdateAsync(UpdateOutletCodeDto dto);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(string outletCode, int? excludeId = null);

        Task<string> GetNextCodeAsync();
    }
}
