using bingGooAPI.Entities;
using bingGooAPI.Models.HouseOpration;

namespace bingGooAPI.Interfaces
{
    public interface IHourOperationRepository
    {
        Task<IEnumerable<HourOperation>> GetAllAsync();

        Task<HourOperation> GetByIdAsync(int id);

        Task<HourOperation> AddAsync(CreateHourOperationDto dto);

        Task<bool> UpdateAsync(UpdateHourOperationDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
