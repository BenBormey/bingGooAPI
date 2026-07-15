using JuJuBiAPI.Entities; 
using JuJuBiAPI.Models.Outlet;

namespace JuJuBiAPI.Interfaces
{
    public interface IOutletRepository
    {
        Task<IEnumerable<OutletListDto>> GetAllAsync();
        Task<OutletListDto?> GetByIdAsync(int id);
        Task<OutletListDto?> GetByCodeAsync(string outletCode);

        Task<Outlet> AddAsync(CreateOutletDtos outletDto);

       
        Task<bool> UpdateAsync(UpdateOutletDto outletDto);
        Task<bool> DeleteAsync(int id);

        Task<bool> OutletExistsAsync(string outletCode);
    }
}