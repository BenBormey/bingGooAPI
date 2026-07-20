using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        // POS lookup: cashiers identify members by phone number.
        Task<IEnumerable<Customer>> SearchAsync(string query);

        Task<int> CreateAsync(Customer customer);

        Task<bool> UpdateAsync(Customer customer);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsByCodeAsync(string customerCode, int? excludeId = null);

        Task<string> GetNextCodeAsync();
    }
}
