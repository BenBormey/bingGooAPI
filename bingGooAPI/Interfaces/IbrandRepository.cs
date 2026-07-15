using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Branch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuJuBiAPI.Interfaces
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Branch>> GetAllAsync();
        Task<Branch?> GetByIdAsync(int id);
        Task<Branch> CreateAsync(CreateBranch model);
        Task<bool> UpdateAsync(Branch model);
        Task<bool> DeleteAsync(int id);
    }
}
