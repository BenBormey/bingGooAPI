using JuJuBiAPI.Entities;
using JuJuBiAPI.Models.ProductDeliveryLogistic;

namespace JuJuBiAPI.Interfaces
{
    public interface IProductDeliveryLogisticRepository
    {
        Task<List<ProductDeliveryLogistic>> GetByProductAsync(string proNumY);

        Task<ProductDeliveryLogistic> CreateAsync(CreateProductDeliveryLogisticDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
