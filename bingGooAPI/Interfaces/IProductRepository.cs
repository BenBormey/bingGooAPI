using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.Product;

namespace JuJuBiAPI.Interfaces
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

        Task<Product?> GetByBarcodeAsync(string barcode);

        Task<bool> UpdateCaseNumberAsync(int id, string? caseNumber);

        Task<bool> UpdateBarcodeAsync(int id, string? barcode);

        Task<bool> UpdateOldBarcodeAsync(int id, string? oldBarcode);

        Task<bool> UpdatePackNumberAsync(int id, string? packNumber);

        Task<bool> UpdateStatusAsync(int id, string? status);
    }
}
