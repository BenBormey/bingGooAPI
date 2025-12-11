using bingGooAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
{
    public interface Ibrand
    {
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch?> GetByIdAsync(int id);
        Task<Branch> CreateAsync(Branch model);
        Task<bool> UpdateAsync(Branch model);
        Task<bool> DeleteAsync(int id);
    }
}
