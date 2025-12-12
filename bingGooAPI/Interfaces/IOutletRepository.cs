using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface IOutletRepository
    {
        Task<IEnumerable<Outlet>> GetAllAsync();
        Task<Outlet?> GetByIdAsync(int id);
        Task<Outlet?> GetByCodeAsync(string outletCode);

        Task AddAsync(Outlet outlet);
        Task UpdateAsync(Outlet outlet);
        Task DeleteAsync(Outlet outlet);

        Task<bool> SaveChangesAsync();
    }
}
