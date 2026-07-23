using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Queries;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Repositories
{
    public class ProductScalRepository : IProductScalRepository
    {
        private readonly IDbConnection _connection;

        public ProductScalRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public async Task<bool> ExistsAsync(string proNumY, string? uomCode, decimal? excludeId = null)
        {
            var count = await _connection.ExecuteScalarAsync<int>(
                ProductScalQueries.Exists,
                new
                {
                    ProNumY = proNumY?.Trim(),
                    UOMCode = uomCode?.Trim(),
                    ExcludeId = excludeId
                });

            return count > 0;
        }

        // Return: new Id if inserted, 0 if duplicate
        public async Task<int> CreateAsync(ProductsScale productScale)
        {
            return await _connection.ExecuteScalarAsync<int>(ProductScalQueries.Create, productScale);
        }

        public async Task<IEnumerable<ProductsScale>> GetAllAsync()
        {
            return await _connection.QueryAsync<ProductsScale>(ProductScalQueries.GetAll);
        }

        public async Task<ProductsScale?> GetByIdAsync(decimal id)
        {
            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                ProductScalQueries.GetById,
                new { Id = id });
        }

        public async Task<ProductsScale?> GetByProductIdAsync(decimal productId)
        {
            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                ProductScalQueries.GetByProductId,
                new { ProductId = productId });
        }

        // Search by ProNumY (partial match — for search box)
        public async Task<IEnumerable<ProductsScale>> SearchByProNumYAsync(string proNumY)
        {
            return await _connection.QueryAsync<ProductsScale>(
                ProductScalQueries.SearchByProNumY,
                new { Search = $"%{proNumY?.Trim()}%" });
        }

        // Exact ProNumY lookup — for barcode scanner
        public async Task<ProductsScale?> GetByProNumYAsync(string proNumY)
        {
            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                ProductScalQueries.GetByProNumY,
                new { ProNumY = proNumY?.Trim() });
        }

        // Return: true = updated, false = not found or duplicate
        public async Task<bool> UpdateAsync(ProductsScale productScale)
        {
            // check duplicate first (excluding this record)
            var duplicate = await ExistsAsync(
                productScale.ProNumY,
                productScale.UOMCode,
                productScale.Id);

            if (duplicate)
                return false;

            var rowsAffected = await _connection.ExecuteAsync(ProductScalQueries.Update, productScale);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var deletedRows = await _connection.ExecuteScalarAsync<int>(ProductScalQueries.Delete, new { Id = id });

            return deletedRows > 0;
        }
    }
}
