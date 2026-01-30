using bingGooAPI.Entities;
using bingGooAPI.Models.Outlet;

namespace bingGooAPI.Interfaces
{
    public interface IOutletRepository
    {
        Task<IEnumerable<OutletListDto>> GetAllAsync();
        Task<OutletListDto?> GetByIdAsync(int id);
        Task<OutletListDto?> GetByCodeAsync(string outletCode);

        Task AddAsync(CreateOutletDtos outlet);
        Task UpdateAsync(UpdateOutletDto outlet);
        Task DeleteAsync(int id );

    }
}
