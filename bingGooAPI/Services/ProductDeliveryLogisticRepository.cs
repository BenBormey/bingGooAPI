using Dapper;
using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.ProductDeliveryLogistic;

using System.Data;

namespace JuJuBiAPI.Services
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
            const string sql = @"
                SELECT
                    d.Id,
                    d.ProNumY,
                    d.ProvinceId,
                    p.ProvinceNameEN,
                    d.AdditionalCost,
                    d.CreatedAt
                FROM ProductDeliveryLogistics d
                JOIN Provinces p ON p.ProvinceId = d.ProvinceId
                WHERE d.ProNumY = @ProNumY
                ORDER BY p.ProvinceNameEN;";

            var list = await _connection.QueryAsync<ProductDeliveryLogistic>(sql, new { ProNumY = proNumY });

            return list.ToList();
        }

        public async Task<ProductDeliveryLogistic> CreateAsync(CreateProductDeliveryLogisticDto dto)
        {
            const string existsSql = @"
                SELECT COUNT(*)
                FROM ProductDeliveryLogistics
                WHERE ProNumY = @ProNumY AND ProvinceId = @ProvinceId;";

            var exists = await _connection.ExecuteScalarAsync<int>(existsSql, new { dto.ProNumY, dto.ProvinceId });

            if (exists > 0)
                throw new InvalidOperationException("This city already has a delivery cost set for this product.");

            const string insertSql = @"
                INSERT INTO ProductDeliveryLogistics
                (
                    ProNumY,
                    ProvinceId,
                    AdditionalCost,
                    CreatedAt
                )
                VALUES
                (
                    @ProNumY,
                    @ProvinceId,
                    @AdditionalCost,
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await _connection.ExecuteScalarAsync<int>(insertSql, dto);

            const string selectSql = @"
                SELECT
                    d.Id,
                    d.ProNumY,
                    d.ProvinceId,
                    p.ProvinceNameEN,
                    d.AdditionalCost,
                    d.CreatedAt
                FROM ProductDeliveryLogistics d
                JOIN Provinces p ON p.ProvinceId = d.ProvinceId
                WHERE d.Id = @Id;";

            return await _connection.QuerySingleAsync<ProductDeliveryLogistic>(selectSql, new { Id = id });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM ProductDeliveryLogistics WHERE Id = @Id;";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
    }
}
