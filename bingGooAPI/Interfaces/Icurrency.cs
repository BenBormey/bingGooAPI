using bingGooAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
{
    public interface ICurrency
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(int id);
        Task<Currency> CreateAsync(Currency model);
        Task<bool> UpdateAsync(Currency model);
        Task<bool> DeleteAsync(int id);
    }
}
