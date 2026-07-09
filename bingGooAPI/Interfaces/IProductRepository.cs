using bingGooAPI.Entities;
using bingGooAPI.Models.Product;

namespace bingGooAPI.Interfaces
{
    public interface IProductRepository
    {
        Task<CreateProductDto> CreateAsync(CreateProductDto product);

        Task<List<ProductListDto>> GetAllAsync();

        Task<ProductListDto?> GetByIdAsync(int id);

        Task<List<ProductListDto>> SearchByNameAsync(string name);

        Task<List<ProductListDto>> SearchBySkuAsync(string sku);

        Task<bool> UpdateAsync(UpdateProductDto product);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
        Task<List<ProductPosDto>> GetForPosAsync(int outletId,int? categoryid);

        Task<Product?> GetByBarcodeAsync(string barcode);
    }
}
