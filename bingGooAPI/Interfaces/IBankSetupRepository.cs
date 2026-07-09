using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IBankSetupRepository
    {
        Task<List<BankSetup>> GetAllAsync();

        Task<BankSetup?> GetByIdAsync(int id);

        Task<int> CreateAsync(BankSetup bank);

        Task<bool> UpdateAsync(BankSetup bank);

        Task<bool> DeleteAsync(int id);
    }
}
