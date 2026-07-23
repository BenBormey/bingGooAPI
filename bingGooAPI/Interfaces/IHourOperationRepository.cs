using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.HouseOpration;

namespace JuJuBiAPI.Interfaces
{
    public interface IHourOperationRepository
    {
        Task<IEnumerable<HourOperation>> GetAllAsync();

        Task<HourOperation?> GetByIdAsync(int id);

        Task<HourOperation> AddAsync(CreateHourOperationDto dto);

        Task<bool> UpdateAsync(UpdateHourOperationDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
