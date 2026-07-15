using JuJuBiAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuJuBiAPI.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(int id);
        Task<Currency> CreateAsync(Currency model);
        Task<bool> UpdateAsync(Currency model);
        Task<bool> DeleteAsync(int id);
    }
}
