using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class ProductScalRepository : IProductScalRepository
    {
        private readonly IDbConnection _connection;

        public ProductScalRepository(IDbConnection connection)
        {
            _connection = connection;
        }

    
        public async Task<bool> ExistsAsync(string proNumY, string uomCode, decimal? excludeId = null)
        {
            const string sql = @"
SELECT COUNT(1)
FROM TblProductsScale
WHERE Status = 1
AND ProNumY = @ProNumY
AND UOMCode = @UOMCode
AND (@ExcludeId IS NULL OR Id <> @ExcludeId);";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    ProNumY = proNumY?.Trim(),
                    UOMCode = uomCode?.Trim(),
                    ExcludeId = excludeId
                });

            return count > 0;
        }

        // Return: new Id បើ insert បាន, 0 បើជាន់គ្នា (duplicate)
        public async Task<int> CreateAsync(ProductsScale productScale)
        {
            const string sql = @"
IF NOT EXISTS
(
    SELECT 1
    FROM TblProductsScale
    WHERE Status = 1
    AND ProNumY = @ProNumY
    AND UOMCode = @UOMCode
)
BEGIN
    INSERT INTO TblProductsScale
    (
        ProId,
        CTNPerPallet,
        UOMCode,
        Width,
        Length,
        Height,
        CBMPerCTN,
        NetWeight,
        GrossWeight,
        CreatedDate,
        Status,
        ProNumY
    )
    VALUES
    (
        @ProId,
        @CTNPerPallet,
        @UOMCode,
        @Width,
        @Length,
        @Height,
        @CBMPerCTN,
        @NetWeight,
        @GrossWeight,
        GETDATE(),
        1,
        @ProNumY
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
ELSE
BEGIN
    SELECT 0;  -- duplicate
END";

            return await _connection.ExecuteScalarAsync<int>(sql, productScale);
        }

        public async Task<IEnumerable<ProductsScale>> GetAllAsync()
        {
            const string sql = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode
WHERE ps.Status = 1
ORDER BY ps.ProNumY;";

            return await _connection.QueryAsync<ProductsScale>(sql);
        }

        public async Task<ProductsScale?> GetByIdAsync(decimal id)
        {
            const string sql = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode
WHERE ps.Id = @Id
AND ps.Status = 1;";

            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                sql,
                new { Id = id });
        }

        public async Task<ProductsScale?> GetByProductIdAsync(decimal productId)
        {
            const string sql = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode
WHERE ps.ProId = @ProductId
AND ps.Status = 1;";

            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                sql,
                new { ProductId = productId });
        }

        // Search by ProNumY (partial match — សម្រាប់ search box)
        public async Task<IEnumerable<ProductsScale>> SearchByProNumYAsync(string proNumY)
        {
            const string sql = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode
WHERE ps.Status = 1
AND ps.ProNumY LIKE @Search
ORDER BY ps.ProNumY;";

            return await _connection.QueryAsync<ProductsScale>(
                sql,
                new { Search = $"%{proNumY?.Trim()}%" });
        }

        // Exact ProNumY lookup — សម្រាប់ barcode scanner
        public async Task<ProductsScale?> GetByProNumYAsync(string proNumY)
        {
            const string sql = @"
SELECT
    ps.Id,
    ps.ProId,
    ps.ProNumY,
    ps.CTNPerPallet,
    ps.UOMCode,
    u.UOMName,
    ps.Width,
    ps.Length,
    ps.Height,
    ps.CBMPerCTN,
    ps.NetWeight,
    ps.GrossWeight,
    ps.CreatedDate,
    ps.Status
FROM TblProductsScale ps
LEFT JOIN UOM u
    ON u.UOMCode = ps.UOMCode
WHERE ps.Status = 1
AND ps.ProNumY = @ProNumY;";

            return await _connection.QueryFirstOrDefaultAsync<ProductsScale>(
                sql,
                new { ProNumY = proNumY?.Trim() });
        }

        // Return: true = updated, false = not found ឬ duplicate
        public async Task<bool> UpdateAsync(ProductsScale productScale)
        {
            // check duplicate ជាមុន (មិនរាប់ record ខ្លួនឯង)
            var duplicate = await ExistsAsync(
                productScale.ProNumY,
                productScale.UOMCode,
                productScale.Id);

            if (duplicate)
                return false;

            const string sql = @"
UPDATE TblProductsScale
SET
    ProId = @ProId,
    CTNPerPallet = @CTNPerPallet,
    UOMCode = @UOMCode,
    Width = @Width,
    Length = @Length,
    Height = @Height,
    CBMPerCTN = @CBMPerCTN,
    NetWeight = @NetWeight,
    GrossWeight = @GrossWeight,
    Status = @Status,
    ProNumY = @ProNumY
WHERE Id = @Id;";

            var rowsAffected = await _connection.ExecuteAsync(sql, productScale);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            const string sql = @"
INSERT INTO TblProductsScaleDelete
(
    ScaleId,
    ProId,
    CTNPerPallet,
    UOMCode,
    Width,
    Length,
    Height,
    CBMPerCTN,
    NetWeight,
    GrossWeight,
    CreatedDate,
    Status,
    ProNumY,
    DeletedDate
)
SELECT
    Id,
    ProId,
    CTNPerPallet,
    UOMCode,
    Width,
    Length,
    Height,
    CBMPerCTN,
    NetWeight,
    GrossWeight,
    CreatedDate,
    Status,
    ProNumY,
    GETDATE()
FROM TblProductsScale
WHERE Id = @Id;

DELETE FROM TblProductsScale
WHERE Id = @Id;

SELECT @@ROWCOUNT;";

            var deletedRows = await _connection.ExecuteScalarAsync<int>(sql, new { Id = id });

            return deletedRows > 0;
        }
    }
}