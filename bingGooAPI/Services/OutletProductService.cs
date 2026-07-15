using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models;
using JuJuBiAPI.Models.OutletProduct;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class OutletProductService : IOutletProductRepository
    {
        private readonly IDbConnection _connection;

        public OutletProductService(IDbConnection connection)
        {
            _connection = connection;
        }

        // NOTE on default columns:
        //   Default sell price -> p.ProUPrSE
        //   Default discount   -> p.ProDis
        // Change these two if your master uses different columns.

        // ---------- Menu grid: every product + this outlet's override ----------
        public async Task<IEnumerable<OutletProductItem>> GetByOutletAsync(int outletId, string? search)
        {
            var sql = @"
                SELECT
                    p.ProNumY                                   AS ProNumY,
                    p.ProID                                     AS ProID,
                    p.ProName                                   AS ProName,
                    p.KhmerNameUnicode                          AS KhmerName,
                    p.ProCat                                    AS Category,
                    ISNULL(op.Currency, p.ProCurr)              AS Currency,

                    ISNULL(p.ProUPrSE, 0)                       AS DefaultSellPrice,
                    ISNULL(p.ProDis, 0)                         AS DefaultDiscount,

                    op.Id                                       AS OutletProductId,
                    ISNULL(op.CanSell, CAST(0 AS bit))          AS CanSell,
                    op.SellPrice                                AS SellPrice,
                    op.DiscountPercent                          AS DiscountPercent,
                    ISNULL(op.IsActive, CAST(1 AS bit))         AS IsActive,

                    ISNULL(op.SellPrice, ISNULL(p.ProUPrSE, 0)) AS EffectiveSellPrice,
                    ISNULL(op.DiscountPercent, ISNULL(p.ProDis, 0)) AS EffectiveDiscount
                FROM TPRProducts p
                LEFT JOIN OutletProduct op
                    ON op.ProNumY = p.ProNumY
                   AND op.OutletId = @OutletId
                WHERE (@Search IS NULL
                        OR p.ProName LIKE '%' + @Search + '%'
                        OR p.ProNumY LIKE '%' + @Search + '%'
                        OR p.KhmerNameUnicode LIKE '%' + @Search + '%')
                ORDER BY p.ProName;";

            return await _connection.QueryAsync<OutletProductItem>(
                sql, new { OutletId = outletId, Search = search });
        }

        // ---------- POS: products this outlet can sell ----------
        public async Task<IEnumerable<SellableProduct>> GetSellableAsync(int outletId)
        {
            var sql = @"
                SELECT
                    p.ProNumY                                       AS ProNumY,
                    p.ProID                                         AS ProID,
                    p.ProName                                       AS ProName,
                    p.KhmerNameUnicode                              AS KhmerName,
                    ISNULL(op.SellPrice, ISNULL(p.ProUPrSE, 0))     AS SellPrice,
                    ISNULL(op.DiscountPercent, ISNULL(p.ProDis, 0)) AS DiscountPercent
                FROM OutletProduct op
                INNER JOIN TPRProducts p ON p.ProNumY = op.ProNumY
                WHERE op.OutletId = @OutletId
                  AND op.CanSell = 1
                  AND op.IsActive = 1
                ORDER BY p.ProName;";

            return await _connection.QueryAsync<SellableProduct>(sql, new { OutletId = outletId });
        }

        // ---------- Upsert one setting ----------
        public async Task<int> UpsertAsync(OutletProductSave model)
        {
            var sql = @"
                MERGE dbo.OutletProduct AS t
                USING (SELECT @OutletId AS OutletId, @ProNumY AS ProNumY) AS s
                    ON (t.OutletId = s.OutletId AND t.ProNumY = s.ProNumY)
                WHEN MATCHED THEN
                    UPDATE SET
                        CanSell         = @CanSell,
                        SellPrice       = @SellPrice,
                        DiscountPercent = @DiscountPercent,
                        DiscountAmount  = @DiscountAmount,
                        Currency        = @Currency,
                        IsActive        = @IsActive,
                        UpdatedAt       = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (OutletId, ProNumY, CanSell, SellPrice, DiscountPercent,
                            DiscountAmount, Currency, IsActive, CreatedAt)
                    VALUES (@OutletId, @ProNumY, @CanSell, @SellPrice, @DiscountPercent,
                            @DiscountAmount, @Currency, @IsActive, GETDATE());";

            return await _connection.ExecuteAsync(sql, model);
        }

        // ---------- Bulk upsert (whole grid) ----------
        public async Task<int> BulkUpsertAsync(IEnumerable<OutletProductSave> items)
        {
            var sql = @"
                MERGE dbo.OutletProduct AS t
                USING (SELECT @OutletId AS OutletId, @ProNumY AS ProNumY) AS s
                    ON (t.OutletId = s.OutletId AND t.ProNumY = s.ProNumY)
                WHEN MATCHED THEN
                    UPDATE SET
                        CanSell         = @CanSell,
                        SellPrice       = @SellPrice,
                        DiscountPercent = @DiscountPercent,
                        DiscountAmount  = @DiscountAmount,
                        Currency        = @Currency,
                        IsActive        = @IsActive,
                        UpdatedAt       = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (OutletId, ProNumY, CanSell, SellPrice, DiscountPercent,
                            DiscountAmount, Currency, IsActive, CreatedAt)
                    VALUES (@OutletId, @ProNumY, @CanSell, @SellPrice, @DiscountPercent,
                            @DiscountAmount, @Currency, @IsActive, GETDATE());";

            // Dapper runs the MERGE once per item in the list
            return await _connection.ExecuteAsync(sql, items);
        }

        // ---------- Toggle CanSell ----------
        public async Task<bool> SetCanSellAsync(int outletId, string proNumY, bool canSell)
        {
            var sql = @"
                MERGE dbo.OutletProduct AS t
                USING (SELECT @OutletId AS OutletId, @ProNumY AS ProNumY) AS s
                    ON (t.OutletId = s.OutletId AND t.ProNumY = s.ProNumY)
                WHEN MATCHED THEN
                    UPDATE SET CanSell = @CanSell, UpdatedAt = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (OutletId, ProNumY, CanSell, IsActive, CreatedAt)
                    VALUES (@OutletId, @ProNumY, @CanSell, 1, GETDATE());";

            var rows = await _connection.ExecuteAsync(
                sql, new { OutletId = outletId, ProNumY = proNumY, CanSell = canSell });
            return rows > 0;
        }

        // ---------- Delete override ----------
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM dbo.OutletProduct WHERE Id = @Id";
            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}