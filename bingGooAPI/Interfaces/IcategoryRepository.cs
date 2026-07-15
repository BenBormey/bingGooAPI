using JuJuBiAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuJuBiAPI.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category model);
        Task<bool> UpdateAsync(Category model);
        Task<bool> DeleteAsync(int id);
    }
}
