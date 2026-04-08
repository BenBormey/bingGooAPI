using bingGooAPI.Interfaces;
using bingGooAPI.Models.ProductStock;
using Dapper;
using System.Data;

namespace bingGooAPI.Services
{
    public class ProductStockService : IProductStockRepository
    {
        private readonly IDbConnection _connection;

        public ProductStockService(IDbConnection connection)
        {
            _connection = connection;
        }

        
        public async Task<int> CreateAsync(CreateProductStockDto dto)
        {
            var sql = @"
                INSERT INTO ProductStocks
                    (ProductID, OutletId, StockQty, LastUpdated)
                VALUES
                    (@ProductID, @OutletId, @StockQty, GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _connection.ExecuteScalarAsync<int>(sql, dto);
        }

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

     
        public async Task<IEnumerable<ProductStockDto>> GetAllAsync()
        {
            var sql = @"
      SELECT  ps.[StockID]
      ,ps.[ProductID]

      ,ps.[StockQty]
      ,ps.[LastUpdated]
      ,ps.[OutletId],
	  o.OutletName,

	  p.ProductName

	  
	  
  FROM [DBAuthentication].[dbo].[ProductStocks] ps

  inner join Outlet o on o.Id = ps.[OutletId]
  inner join Products p on p.ProductID = ps.ProductID

                ORDER BY  ps.StockID DESC
            ";

            return await _connection.QueryAsync<ProductStockDto>(sql);
        }

       
        public async Task<ProductStockDto> GetByIdAsync(int stockId)
        {
            var sql = @"
      SELECT  ps.[StockID]
      ,ps.[ProductID]

      ,ps.[StockQty]
      ,ps.[LastUpdated]
      ,ps.[OutletId],
	  o.OutletName,

	  p.ProductName

	  
	  
  FROM [DBAuthentication].[dbo].[ProductStocks] ps
  
  inner join Outlet o on o.Id = ps.[OutletId]
  inner join Products p on p.ProductID = ps.ProductID
                WHERE ps.[StockID] = @StockID
            ";

            return await _connection.QueryFirstOrDefaultAsync<ProductStockDto>(
                sql,
                new { StockID = stockId }
            );
        }

    
        public async Task<ProductStockDto> GetByProductBranchOutletAsync(
            int productId,
  
            int outletId)
        {
            var sql = @"
  SELECT  ps.[StockID]
      ,ps.[ProductID]
 
      ,ps.[StockQty]
      ,ps.[LastUpdated]
      ,ps.[OutletId],
	  o.OutletName,
	
	  p.ProductName

	  
	  
  FROM [DBAuthentication].[dbo].[ProductStocks] ps

  inner join Outlet o on o.Id = ps.[OutletId]
  inner join Products p on p.ProductID = ps.ProductID
                WHERE ProductID = @ProductID
               
                  AND OutletId = @OutletId
            ";

            return await _connection.QueryFirstOrDefaultAsync<ProductStockDto>(
                sql,
                new
                {
                    ProductID = productId,
                 
                    OutletId = outletId
                }
            );
        }

        public async Task<bool> UpdateAsync(UpdateProductStockDto dto)
        {
            var sql = @"
                UPDATE ProductStocks
                SET
                    ProductID = @ProductID,
                    OutletId = @OutletId,
                    StockQty = @StockQty,
                    LastUpdated = GETDATE()
                WHERE StockID = @StockID
            ";

            var rows = await _connection.ExecuteAsync(sql, dto);

            return rows > 0;
        }
    }
}
