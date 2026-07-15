using JuJuBiAPI.Entities;

namespace JuJuBiAPI.Interfaces
{
    public interface IProductScalRepository
    {
        Task<int> CreateAsync(ProductsScale productScale);
        Task<IEnumerable<ProductsScale>> GetAllAsync();
        Task<ProductsScale?> GetByIdAsync(decimal id);
        Task<ProductsScale?> GetByProductIdAsync(decimal productId);
        Task<IEnumerable<ProductsScale>> SearchByProNumYAsync(string proNumY);
        Task<ProductsScale?> GetByProNumYAsync(string proNumY);
        Task<bool> ExistsAsync(string proNumY, string uomCode, decimal? excludeId = null);
        Task<bool> UpdateAsync(ProductsScale productScale);
        Task<bool> DeleteAsync(decimal id);
    }
}