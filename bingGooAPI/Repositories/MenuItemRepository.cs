using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using Dapper;
using JuJuBis.Domain.Entities;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly IDbConnection _connection;

        public MenuItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            var sql = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(PS.StockQty, 0) AS StockQty,
        CAT.Id AS CategoryId,
        CAT.CategoryName,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN Products PR ON PR.ProductCode = M.ProNumY
        LEFT JOIN ProductStocks PS ON PS.ProductID = PR.ProductID AND PS.OutletId = M.OutletId
        LEFT JOIN Category CAT ON (CAT.CategoryCode = P.ProCat OR CAT.Id = TRY_CAST(P.ProCat AS INT))
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id
    ORDER BY M.MenuItemId DESC";

            return await _connection.QueryAsync<MenuItem>(sql);
        }
        public async Task<MenuItem?> GetByIdAsync(int menuItemId)
        {
            var sql = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(PS.StockQty, 0) AS StockQty,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN Products PR ON PR.ProductCode = M.ProNumY
        LEFT JOIN ProductStocks PS ON PS.ProductID = PR.ProductID AND PS.OutletId = M.OutletId
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id
    WHERE M.MenuItemId=@MenuItemId";

            return await _connection.QueryFirstOrDefaultAsync<MenuItem>(
                sql,
                new { MenuItemId = menuItemId });
        }
        public async Task<IEnumerable<MenuItem>> GetByOutletAsync(int outletId)
        {
            var sql = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(PS.StockQty, 0) AS StockQty,
        CAT.Id AS CategoryId,
        CAT.CategoryName,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
        LEFT JOIN Products PR ON PR.ProductCode = M.ProNumY
        LEFT JOIN ProductStocks PS ON PS.ProductID = PR.ProductID AND PS.OutletId = M.OutletId
        LEFT JOIN Category CAT ON (CAT.CategoryCode = P.ProCat OR CAT.Id = TRY_CAST(P.ProCat AS INT))
        INNER JOIN Outlet O ON M.OutletId=O.Id
        INNER JOIN Currency C ON M.CurrencyId=C.Id
    WHERE M.OutletId=@OutletId
    ORDER BY P.ProName";

            return await _connection.QueryAsync<MenuItem>(
                sql,
                new { OutletId = outletId });
        }

        public async Task<MenuItem?> GetByOutletAndProductAsync(int outletId, string proNumY)
        {
            var sql = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(PS.StockQty, 0) AS StockQty,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
        LEFT JOIN Products PR ON PR.ProductCode = M.ProNumY
        LEFT JOIN ProductStocks PS ON PS.ProductID = PR.ProductID AND PS.OutletId = M.OutletId
        INNER JOIN Outlet O ON M.OutletId=O.Id
        INNER JOIN Currency C ON M.CurrencyId=C.Id
    WHERE M.OutletId=@OutletId
      AND M.ProNumY=@ProNumY";

            return await _connection.QueryFirstOrDefaultAsync<MenuItem>(
                sql,
                new
                {
                    OutletId = outletId,
                    ProNumY = proNumY
                });
        }
        public async Task<bool> ExistsAsync(int outletId, string proNumY)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM MenuItem
                WHERE OutletId = @OutletId
                  AND ProNumY = @ProNumY";

            var count = await _connection.ExecuteScalarAsync<int>(
                sql,
                new
                {
                    OutletId = outletId,
                    ProNumY = proNumY
                });

            return count > 0;
        }

        public async Task<MenuItem> CreateAsync(MenuItem model)
        {
            var sql = @"
    INSERT INTO MenuItem
    (
        OutletId,
        ProNumY,
        CurrencyId,
        SellingPrice,
        IsPromotion,
        Discount,
        PromotionPrice,
        PromoStartDate,
        PromoEndDate,
        IsActive,
        ImageFileName,
        Remark,
        CreatedBy
    )
    VALUES
    (
        @OutletId,
        @ProNumY,
        @CurrencyId,
        @SellingPrice,
        @IsPromotion,
        @Discount,
        @PromotionPrice,
        @PromoStartDate,
        @PromoEndDate,
        @IsActive,
        @ImageFileName,
        @Remark,
        @CreatedBy
    );

    SELECT
        M.*,
        P.ProName AS ProductName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(PS.StockQty, 0) AS StockQty,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
        LEFT JOIN Products PR ON PR.ProductCode = M.ProNumY
        LEFT JOIN ProductStocks PS ON PS.ProductID = PR.ProductID AND PS.OutletId = M.OutletId
        INNER JOIN Outlet O ON M.OutletId=O.Id
        INNER JOIN Currency C ON M.CurrencyId=C.Id
    WHERE M.MenuItemId=CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.QuerySingleAsync<MenuItem>(sql, model);
        }

        public async Task<bool> UpdateAsync(MenuItem model)
        {
            var sql = @"
    UPDATE MenuItem
    SET
        CurrencyId=@CurrencyId,
        SellingPrice=@SellingPrice,
        IsPromotion=@IsPromotion,
        Discount=@Discount,
        PromotionPrice=@PromotionPrice,
        PromoStartDate=@PromoStartDate,
        PromoEndDate=@PromoEndDate,
        IsActive=@IsActive,
        ImageFileName=@ImageFileName,
        Remark=@Remark,
        UpdatedBy=@UpdatedBy,
        UpdatedAt=GETDATE()
    WHERE MenuItemId=@MenuItemId";

            return await _connection.ExecuteAsync(sql, model) > 0;
        }

        public async Task<bool> DeleteAsync(int menuItemId)
        {
            var sql = @"
                DELETE FROM MenuItem
                WHERE MenuItemId = @MenuItemId";

            var rows = await _connection.ExecuteAsync(
                sql,
                new { MenuItemId = menuItemId });

            return rows > 0;
        }

        // Applies one promotion percentage across every menu item in an outlet.
        // PromotionPrice is stored alongside the percent so the price stays fixed
        // even if the selling price is edited later.
        public async Task<int> SetOutletDiscountAsync(
            int outletId, decimal percent, DateTime? startDate, DateTime? endDate, string updatedBy)
        {
            var sql = @"
                UPDATE MenuItem
                SET IsPromotion    = 1,
                    Discount       = @Percent,
                    PromotionPrice = ROUND(SellingPrice * (1 - @Percent / 100.0), 2),
                    PromoStartDate = @StartDate,
                    PromoEndDate   = @EndDate,
                    UpdatedBy      = @UpdatedBy,
                    UpdatedAt      = GETDATE()
                WHERE OutletId = @OutletId;";

            return await _connection.ExecuteAsync(sql, new
            {
                OutletId = outletId,
                Percent = percent,
                StartDate = startDate,
                EndDate = endDate,
                UpdatedBy = updatedBy
            });
        }

        public async Task<int> ClearOutletDiscountAsync(int outletId, string updatedBy)
        {
            var sql = @"
                UPDATE MenuItem
                SET IsPromotion    = 0,
                    Discount       = NULL,
                    PromotionPrice = NULL,
                    PromoStartDate = NULL,
                    PromoEndDate   = NULL,
                    UpdatedBy      = @UpdatedBy,
                    UpdatedAt      = GETDATE()
                WHERE OutletId = @OutletId;";

            return await _connection.ExecuteAsync(sql, new
            {
                OutletId = outletId,
                UpdatedBy = updatedBy
            });
        }

        public async Task<bool> SetActiveAsync(int menuItemId, bool isActive, string updatedBy)
        {
            var sql = @"
                UPDATE MenuItem
                SET
                    IsActive = @IsActive,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = GETDATE()
                WHERE MenuItemId = @MenuItemId";

            var rows = await _connection.ExecuteAsync(
                sql,
                new
                {
                    MenuItemId = menuItemId,
                    IsActive = isActive,
                    UpdatedBy = updatedBy
                });

            return rows > 0;
        }
    }
}