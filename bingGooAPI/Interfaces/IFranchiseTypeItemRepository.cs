using JuJuBiAPI.Entities; // ឬ JuJuBiAPI.Models ទៅតាមទីតាំងរបស់ FranchiseType Class
using JuJuBiAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuJuBiAPI.Interfaces
{
    public interface IFranchiseTypeItemRepository
    {
      
        Task<FranchiseType> CreateAsync(FranchiseType model);


        Task<IEnumerable<FranchiseType>> GetAllAsync();

  
        Task<FranchiseType?> GetByIdAsync(int id);


        Task<bool> UpdateAsync(FranchiseType model);

        Task<bool> DeleteAsync(int id);
    }
}