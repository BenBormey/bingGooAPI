using bingGooAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
{
    public interface IcategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category model);
        Task<bool> UpdateAsync(Category model);
        Task<bool> DeleteAsync(int id);
    }
}
