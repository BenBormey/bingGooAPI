using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Product;
using Dapper;
using System.Data;
using System.Linq;

namespace bingGooAPI.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<Product> CreateAsync(Product product)
        {
            var sql = @"
INSERT INTO Products
(
    ProductCode,
    ProductName,
    BrandID,
    CategoryId,
    SupplierId,
    ImageUrl,
    CostPrice,
    SellPrice,
    DiscountPercent,
    DiscountAmount,
    TaxPercent,
    Status,
    CreatedAt
)
VALUES
(
    @ProductCode,
    @ProductName,
    @BrandID,
    @CategoryId,
    @SupplierId,
    @ImageUrl,
    @CostPrice,
    @SellingPrice,
    @DiscountPercent,
    @DiscountAmount,
    @TaxPercent,
    @Status,
    GETDATE()
);

SELECT CAST(SCOPE_IDENTITY() as int);
";

            var id = await _connection.ExecuteScalarAsync<int>(sql, product);

            product.ProductID = id;

            return product;
        }
        public async Task<List<ProductListDto>> GetAllAsync()
        {
            var sql = @"
SELECT 
    p.ProductID,
    p.ProductCode,
    p.ProductName,
    p.BrandID,
    b.BranchName AS BrandName,
    p.CategoryId,
    c.CategoryName,
    p.SupplierId,
    s.SupplierName,
    p.ImageUrl,
    p.CostPrice,
    p.SellPrice as SellingPrice,
    p.DiscountPercent,
    p.DiscountAmount,
    p.TaxPercent,
    p.Status,
    p.CreatedAt,
    p.UpdatedAt

FROM Products p
INNER JOIN Suppliers s ON s.SupplierID = p.SupplierId
INNER JOIN Branch b ON b.Id = p.BrandID
INNER JOIN Category c ON c.Id = p.CategoryId
where p.Status = 1

;
";

            var data = await _connection.QueryAsync<ProductListDto>(sql);

            return data.ToList();
        }

 

        public async Task<Product?> GetByIdAsync(int id)
        {
            var sql = @"SELECT * FROM Products WHERE ProductID = @Id";

            return await _connection.QueryFirstOrDefaultAsync<Product>(
                sql,
                new { Id = id }
            );
        }


        //SellingPrice
        public async Task<bool> UpdateAsync(Product product)
        {
            var sql = @"
UPDATE Products SET

    ProductCode = @ProductCode,
    ProductName = @ProductName,
    BrandID = @BrandID,
    CategoryId = @CategoryId,
    SupplierId = @SupplierId,
    ImageUrl = @ImageUrl,
    CostPrice = @CostPrice,
    SellPrice = @SellingPrice,
    DiscountPercent = @DiscountPercent,
    DiscountAmount = @DiscountAmount,
    TaxPercent = @TaxPercent,
    Status = @Status,
    UpdatedAt = GETDATE()

WHERE ProductID = @ProductID;
";

            var rows = await _connection.ExecuteAsync(sql, product);

            return rows > 0;
        }



        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
UPDATE Products
SET Status = 0,
    UpdatedAt = GETDATE()
WHERE ProductID = @Id;
";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }



        public async Task<bool> ExistsAsync(int id)
        {
            var sql = @"SELECT COUNT(1) FROM Products WHERE ProductID = @Id";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql,
                new { Id = id }
            );

            return count > 0;
        }

        public async Task<List<ProductPosDto>> GetForPosAsync(
     int outletId,
     int? categoryId
 )
        {
            var sql = @"
SELECT 
    p.ProductID,
    p.ProductCode,
    p.ProductName,

    p.BrandID,
    b.BranchName AS BrandName,

    p.CategoryId,
    c.CategoryName,

    p.SupplierId,
    s.SupplierName,

    p.ImageUrl,
    p.CostPrice,
    p.SellPrice AS SellingPrice,

    p.DiscountPercent,
    p.DiscountAmount,
    p.TaxPercent,

    p.Status,
    p.CreatedAt,
    p.UpdatedAt,

    ISNULL(ps.StockQty, 0) AS StockQty,
    o.OutletName

FROM Products p
INNER JOIN Suppliers s ON s.SupplierID = p.SupplierId
INNER JOIN Branch b ON b.Id = p.BrandID
INNER JOIN Category c ON c.Id = p.CategoryId
INNER JOIN ProductStocks ps 
    ON ps.ProductID = p.ProductID
    AND ps.OutletId = @OutletId
INNER JOIN Outlet o ON o.Id = ps.OutletId

WHERE 
    p.Status = 1
    AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)

ORDER BY p.ProductName;
";

            var data = await _connection.QueryAsync<ProductPosDto>(
                sql,
                new
                {
                    OutletId = outletId,
                    CategoryId = categoryId
                });

            return data.ToList();
        }


    }
}
