using bingGooAPI.Entities;
using bingGooAPI.Models.Branch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
{
    public interface IbrandRepository
    {
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch?> GetByIdAsync(int id);
        Task<Branch> CreateAsync(CreateBranch model);
        Task<bool> UpdateAsync(Branch model);
        Task<bool> DeleteAsync(int id);
    }
}
