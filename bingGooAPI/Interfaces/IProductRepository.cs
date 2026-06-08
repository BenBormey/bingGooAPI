using bingGooAPI.Entities;
using bingGooAPI.Models.Product;

namespace bingGooAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<CreateProductDto> CreateAsync(CreateProductDto product);

        Task<List<ProductListDto>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(Product product);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
        Task<List<ProductPosDto>> GetForPosAsync(int outletId,int? categoryid);
    }
}
