namespace JuJuBiAPI.Queries
{
    // Raw SQL for MenuItemRepository, kept separate so the repository itself
    // only has to read as call -> map, not call -> map with a script in between.
    public static class MenuItemQueries
    {
        private const string SelectColumns = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        P.ProImage AS ImageFileName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(OS.StockQty, 0) AS StockQty,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN OutletStock OS ON OS.ProNumY = M.ProNumY AND OS.OutletId = M.OutletId
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id";

        public const string GetAll = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        P.ProImage AS ImageFileName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(OS.StockQty, 0) AS StockQty,
        CAT.Id AS CategoryId,
        CAT.CategoryName,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN OutletStock OS ON OS.ProNumY = M.ProNumY AND OS.OutletId = M.OutletId
        LEFT JOIN Category CAT ON (CAT.CategoryCode = P.ProCat OR CAT.Id = TRY_CAST(P.ProCat AS INT))
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id
    ORDER BY M.MenuItemId DESC";

        public const string GetById = SelectColumns + @"
    WHERE M.MenuItemId = @MenuItemId";

        public const string GetByOutlet = @"
    SELECT
        M.*,
        P.ProName AS ProductName,
        P.ProImage AS ImageFileName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(OS.StockQty, 0) AS StockQty,
        CAT.Id AS CategoryId,
        CAT.CategoryName,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN OutletStock OS ON OS.ProNumY = M.ProNumY AND OS.OutletId = M.OutletId
        LEFT JOIN Category CAT ON (CAT.CategoryCode = P.ProCat OR CAT.Id = TRY_CAST(P.ProCat AS INT))
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id
    WHERE M.OutletId = @OutletId
    ORDER BY P.ProName";

        public const string GetByOutletAndProduct = SelectColumns + @"
    WHERE M.OutletId = @OutletId
      AND M.ProNumY = @ProNumY";

        public const string Exists = @"
    SELECT COUNT(1)
    FROM MenuItem
    WHERE OutletId = @OutletId
      AND ProNumY = @ProNumY";

        // INSERT + SELECT-back must stay in one batch so SCOPE_IDENTITY() sees
        // the row this call just inserted, not a later/unrelated insert.
        public const string Create = @"
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
        P.ProImage AS ImageFileName,
        ISNULL(P.ProImpPri, 0) AS CostPrice,
        ISNULL(OS.StockQty, 0) AS StockQty,
        O.OutletName,
        C.CurrencyCode,
        C.CurrencyName
    FROM MenuItem M
        INNER JOIN TPRProducts P ON M.ProNumY = P.ProNumY
        LEFT JOIN OutletStock OS ON OS.ProNumY = M.ProNumY AND OS.OutletId = M.OutletId
        INNER JOIN Outlet O ON M.OutletId = O.Id
        INNER JOIN Currency C ON M.CurrencyId = C.Id
    WHERE M.MenuItemId = CAST(SCOPE_IDENTITY() AS INT);";

        public const string Update = @"
    UPDATE MenuItem
    SET
        CurrencyId = @CurrencyId,
        SellingPrice = @SellingPrice,
        IsPromotion = @IsPromotion,
        Discount = @Discount,
        PromotionPrice = @PromotionPrice,
        PromoStartDate = @PromoStartDate,
        PromoEndDate = @PromoEndDate,
        IsActive = @IsActive,
        ImageFileName = @ImageFileName,
        Remark = @Remark,
        UpdatedBy = @UpdatedBy,
        UpdatedAt = GETDATE()
    WHERE MenuItemId = @MenuItemId";

        public const string Delete = @"
    DELETE FROM MenuItem
    WHERE MenuItemId = @MenuItemId";

        // Applies one promotion percentage across every menu item in an outlet.
        // PromotionPrice is stored alongside the percent so the price stays fixed
        // even if the selling price is edited later.
        public const string SetOutletDiscount = @"
    UPDATE MenuItem
    SET IsPromotion    = 1,
        Discount       = @Percent,
        PromotionPrice = ROUND(SellingPrice * (1 - @Percent / 100.0), 2),
        PromoStartDate = @StartDate,
        PromoEndDate   = @EndDate,
        UpdatedBy      = @UpdatedBy,
        UpdatedAt      = GETDATE()
    WHERE OutletId = @OutletId;";

        public const string ClearOutletDiscount = @"
    UPDATE MenuItem
    SET IsPromotion    = 0,
        Discount       = NULL,
        PromotionPrice = NULL,
        PromoStartDate = NULL,
        PromoEndDate   = NULL,
        UpdatedBy      = @UpdatedBy,
        UpdatedAt      = GETDATE()
    WHERE OutletId = @OutletId;";

        public const string SetActive = @"
    UPDATE MenuItem
    SET
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedAt = GETDATE()
    WHERE MenuItemId = @MenuItemId";
    }
}
