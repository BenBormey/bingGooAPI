using bingGooAPI.Entities;
using bingGooAPI.Models.Term;

namespace bingGooAPI.Interfaces
{
    public interface ITermDayRepository
    {
        Task<IEnumerable<TermDayEntity>> GetAllAsync();

        Task<TermDayEntity?> GetByIdAsync(int id);

        Task<TermDayEntity> AddAsync(CreateTermDayDto dto);

        Task<bool> UpdateAsync(UpdateTermDayDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
