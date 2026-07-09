using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IUomRepository
    {
        Task<IEnumerable<UOM>> GetAllAsync();

        Task<UOM?> GetByIdAsync(int id);

        Task<int> CreateAsync(UOM uom);

        Task<bool> UpdateAsync(UOM uom);

        Task<bool> DeleteAsync(int id);
    }
}