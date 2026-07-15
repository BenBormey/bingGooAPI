using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetAllAsync();
        Task<ExchangeRate?> GetByIdAsync(int id);
        Task<ExchangeRate> CreateAsync(ExchangeRate model);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ExchangeRate>> GetByDateAsync(DateTime date);

    }
}
