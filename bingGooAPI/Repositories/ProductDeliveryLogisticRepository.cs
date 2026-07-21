using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.ProductDeliveryLogistic;
using JuJuBiAPI.Queries;

using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class ProductDeliveryLogisticRepository : IProductDeliveryLogisticRepository
    {
        private readonly IDbConnection _connection;

        public ProductDeliveryLogisticRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<ProductDeliveryLogistic>> GetByProductAsync(string proNumY)
        {
            var list = await _connection.QueryAsync<ProductDeliveryLogistic>(
                ProductDeliveryLogisticQueries.GetByProduct, new { ProNumY = proNumY });

            return list.ToList();
        }

        public async Task<ProductDeliveryLogistic> CreateAsync(CreateProductDeliveryLogisticDto dto)
        {
            var exists = await _connection.ExecuteScalarAsync<int>(
                ProductDeliveryLogisticQueries.Exists, new { dto.ProNumY, dto.ProvinceId });

            if (exists > 0)
                throw new InvalidOperationException("This city already has a delivery cost set for this product.");

            var id = await _connection.ExecuteScalarAsync<int>(ProductDeliveryLogisticQueries.Insert, dto);

            return await _connection.QuerySingleAsync<ProductDeliveryLogistic>(
                ProductDeliveryLogisticQueries.GetById, new { Id = id });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _connection.ExecuteAsync(ProductDeliveryLogisticQueries.Delete, new { Id = id });

            return rows > 0;
        }
    }
}
