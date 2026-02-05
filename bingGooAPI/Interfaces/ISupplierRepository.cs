using bingGooAPI.Entities;

namespace bingGooAPI.Interfaces
{
    public interface ISupplierRepository
    {
 
        Task<IEnumerable<Supplier>> GetAllAsync();

        Task<Supplier?> GetByIdAsync(int id);

        Task<Supplier> CreateAsync(Supplier supplier);

 
        Task<bool> UpdateAsync(Supplier supplier);

        Task<bool> DeleteAsync(int id);
    }
}
