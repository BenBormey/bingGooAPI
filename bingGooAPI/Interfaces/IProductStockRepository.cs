namespace bingGooAPI.Interfaces
{
    using bingGooAPI.Models.ProductStock;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProductStockRepository
    {

        Task<int> CreateAsync(CreateProductStockDto dto);


        Task<IEnumerable<ProductStockDto>> GetAllAsync();

        Task<ProductStockDto> GetByIdAsync(int stockId);

        Task<ProductStockDto> GetByProductAndBranchAsync(int productId, int branchId);


        Task<bool> UpdateAsync(UpdateProductStockDto dto);


        Task<bool> DeleteAsync(int stockId);
    }
}
