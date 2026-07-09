using bingGooAPI.Entities;
using bingGooAPI.Models.ShelfLife;

namespace bingGooAPI.Interfaces
{
    public interface IShelfLifeRepository
    {
        Task<IEnumerable<ShelfLifeEntity>> GetAllAsync();

        Task<ShelfLifeEntity?> GetByIdAsync(int id);

        Task<ShelfLifeEntity> AddAsync(CreateShelfLifeDto dto);

        Task<bool> UpdateAsync(UpdateShelfLifeDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
