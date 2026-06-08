using bingGooAPI.Entities; // ឬ bingGooAPI.Models ទៅតាមទីតាំងរបស់ FranchiseType Class
using bingGooAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
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