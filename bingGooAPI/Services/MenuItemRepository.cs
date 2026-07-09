using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using Dapper;
using JuJuBis.Domain.Entities;
using System.Data;

namespace bingGooAPI.Services
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
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
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
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
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
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
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
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
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
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY=P.ProNumY
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