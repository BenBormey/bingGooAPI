using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Term;

namespace JuJuBiAPI.Interfaces
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
