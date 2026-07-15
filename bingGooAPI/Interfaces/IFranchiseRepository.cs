using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IFranchiseRepository
    {
        Task<List<Franchise>> GetAllAsync();

        Task<Franchise> GetByIdAsync(int id);

        Task<int> InsertAsync(Franchise franchise);

        Task<int> UpdateAsync(Franchise franchise);

        Task<int> DeleteAsync(int id);
    }
}
