using bingGooAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bingGooAPI.Interfaces
{
    public interface IProvincesRepository
    {
        Task<IEnumerable<Provinces>> GetAllProvincesAsync();

        Task<Provinces?> GetProvinceByIdAsync(int id);

        Task<Provinces> CreateAsync(Provinces model);

        Task<bool> UpdateAsync(Provinces model);

        Task<bool> DeleteAsync(int id);
    }
}