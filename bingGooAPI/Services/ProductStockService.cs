using bingGooAPI.Interfaces;
using bingGooAPI.Models.ProductStock;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class ProductService : IProductStockRepository
    {
        private readonly IDbConnection _connection;

        public ProductService(IDbConnection connection)
        {
            _connection = connection;
        }

        // CREATE
        public async Task<int> CreateAsync(CreateProductStockDto dto)
        {
            var sql = @"
                INSERT INTO ProductStocks
                    (ProductID, BranchId, StockQty, LastUpdated)
                VALUES
                    (@ProductID, @BranchId, @StockQty, GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.ExecuteScalarAsync<int>(sql, dto);
        }

        // DELETE
        public async Task<bool> DeleteAsync(int stockId)
        {
            var sql = @"
                DELETE FROM ProductStocks
                WHERE StockID = @StockID
            ";

            var rows = await _connection.ExecuteAsync(sql, new
            {
                StockID = stockId
            });

            return rows > 0;
        }

        // GET ALL
        public async Task<IEnumerable<ProductStockDto>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    StockID,
                    ProductID,
                    BranchId,
                    StockQty,
                    LastUpdated
                FROM ProductStocks
                ORDER BY StockID DESC
            ";

            return await _connection.QueryAsync<ProductStockDto>(sql);
        }

        // GET BY ID
        public async Task<ProductStockDto> GetByIdAsync(int stockId)
        {
            var sql = @"
                SELECT
                    StockID,
                    ProductID,
                    BranchId,
                    StockQty,
                    LastUpdated
                FROM ProductStocks
                WHERE StockID = @StockID
            ";

            return await _connection.QueryFirstOrDefaultAsync<ProductStockDto>(
                sql,
                new { StockID = stockId }
            );
        }

        // GET BY PRODUCT + BRANCH
        public async Task<ProductStockDto> GetByProductAndBranchAsync(int productId, int branchId)
        {
            var sql = @"
                SELECT
                    StockID,
                    ProductID,
                    BranchId,
                    StockQty,
                    LastUpdated
                FROM ProductStocks
                WHERE ProductID = @ProductID
                  AND BranchId = @BranchId
            ";

            return await _connection.QueryFirstOrDefaultAsync<ProductStockDto>(
                sql,
                new
                {
                    ProductID = productId,
                    BranchId = branchId
                }
            );
        }

        // UPDATE
        public async Task<bool> UpdateAsync(UpdateProductStockDto dto)
        {
            var sql = @"
                UPDATE ProductStocks
                SET
                    ProductID = @ProductID,
                    BranchId = @BranchId,
                    StockQty = @StockQty,
                    LastUpdated = GETDATE()
                WHERE StockID = @StockID
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);

            return rows > 0;
        }
    }
}
