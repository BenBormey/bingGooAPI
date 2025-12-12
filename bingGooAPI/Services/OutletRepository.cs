using bingGooAPI.Entities;
using bingGooAPI.Interfaces;

namespace bingGooAPI.Services
{
    public class OutletRepository : IOutletRepository
    {
        public Task AddAsync(Outlet outlet)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Outlet outlet)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Outlet>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Outlet?> GetByCodeAsync(string outletCode)
        {
            throw new NotImplementedException();
        }

        public Task<Outlet?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Outlet outlet)
        {
            throw new NotImplementedException();
        }
    }
}
