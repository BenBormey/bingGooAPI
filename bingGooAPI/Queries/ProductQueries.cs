namespace JuJuBiAPI.Queries
{
    public static class ProductQueries
    {
        public const string ExistsByBarcodes = @"SELECT COUNT(*)
              FROM [DBJuJuBi].[dbo].[TPRProducts]
              WHERE ProNumY = @ProNumY
                 OR (@ProNumYP <> '' AND ProNumYP = @ProNumYP)
                 OR (@ProNumYC <> '' AND ProNumYC = @ProNumYC)";

        public const string Insert = @"
INSERT INTO [DBJuJuBi].[dbo].[TPRProducts]
(
    ProNumY, ProNumS, ProNumYP, ProNumYC,
    Sup1, Sup2,
    ProName, KhmerNameUnicode,
  ProDes, ProCat, ProPacksize,
    ProCurr, ProImpPri,
    ProRecLev, ProRecOrder,
    KhmerName, ProRem,
    Auto, ProfitAuto,
    ProTotQty, ProMadein,
    ProQtyPCase, ProQtyPPack,
    ProPckPri, ProPckDis, ProPckAllDis,
    ProRecomLev, Promotion,
    FormDLanded, ProUPriBY,

    ProDis, ExciseTax,
    PublicLightingTax, ProVAT,
    ProFinBuyin, ProUPrSE,
    ProProPer, ProUPriSeH,
    ProHolesaleper, ProHoleSalePP,
    ProRecPer, ProSKU,
    Average, BirthDate,
    AverSalePmonth, WHcode,
    Sampling, FactoryCurrency,
    FOB_CIF, FOBCIFCost,
    ShelfLifeOfProduct,
    VOP,
    ProImage
)
VALUES
(
    @ProNumY, @ProNumS, @ProNumYP, @ProNumYC,
    ISNULL(@Sup1, ''), @Sup2,
    ISNULL(@ProName, ''), @KhmerNameUnicode,
    @ProDes, ISNULL(@ProCat, ''), ISNULL(@ProPacksize, ''),
    ISNULL(@ProCurr, ''), ISNULL(@ProImpPri, 0),
    ISNULL(@ProRecLev, 0), ISNULL(@ProRecOrder, 0),
    @KhmerName, @ProRem,
    @Auto, @ProfitAuto,
    ISNULL(@ProTotQty, 0), @ProMadein,
    CASE WHEN ISNULL(@ProQtyPCase, 0) = 0 THEN 1 ELSE @ProQtyPCase END,
    @ProQtyPPack,
    @ProPckPri, @ProPckDis, ISNULL(@ProPckAllDis, 0),
    @ProRecomLev, ISNULL(@Promotion, 0),
    ISNULL(@FormDLanded, 0), @ProUPriBY,

    ISNULL(@ProDis, 0), @ExciseTax,
    @PublicLightingTax, ISNULL(@ProVAT, 0),
    ISNULL(@ProFinBuyin, 0), ISNULL(@ProUPrSE, 0),
    @ProProPer, @ProUPriSeH,
    ISNULL(@ProHolesaleper, 0), @ProHoleSalePP,
    @ProRecPer, @ProSKU,
    @Average, @BirthDate,
    ISNULL(@AverSalePmonth, 0), @WHcode,
    @Sampling, @FactoryCurrency,
    @FOB_CIF, @FOBCIFCost,
    @ShelfLifeOfProduct,
    @VOP,
    @ProImage
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public const string InsertScale = @"
INSERT INTO [DBJuJuBi].[dbo].[TblProductsScale]
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
    Status
)
VALUES
(
    @ProId,
    ISNULL(@CTNPerPallet, 0),
    ISNULL(@UOMCode, ''),
    ISNULL(@Width, 0),
    ISNULL(@Length, 0),
    ISNULL(@Height, 0),
    ISNULL(@CBMPerCTN, 0),
    ISNULL(@NetWeight, 0),
    ISNULL(@GrossWeight, 0),
    @CreatedDate,
    @Status
);";

        // Shared SELECT + joins for the list/search queries (identical columns).
        private const string ListSelect = @"
SELECT
    p.ProID,
    p.ProNumY,
    p.ProNumS,
    p.ProNumYP,
		c.Id AS categoryId,
    p.ProNumYC,
    p.Sup1,
    p.Sup2,
    p.ProName,
    p.KhmerNameUnicode,
    p.ProDes,
    p.ProCat,
    c.CategoryName,
    p.ProPacksize,
    p.ProCurr,
    p.ProImpPri,
    p.ProRecLev,
    p.ProRecOrder,
    p.KhmerName,
    p.ProRem,
    p.Auto,
    p.ProfitAuto,
    p.ProTotQty,
    p.ProMadein,
    p.ProQtyPCase,
    p.ProQtyPPack,
    p.ProPckPri,
    p.ProPckDis,
    p.ProPckAllDis,
    p.ProRecomLev,
    p.Promotion,
    p.FormDLanded,
    p.ProUPriBY,
    p.ProAllowDisW,
    p.ProAllowDisU,
    p.ProDis,
    p.ExciseTax,
    p.PublicLightingTax,
    p.ProVAT,
    p.ProFinBuyin,
    p.ProUPrSE,
    p.ProProPer,
    p.ProUPriSeH,
    p.ProHolesaleper,
    p.ProHoleSalePP,
    p.ProRecPer,
    p.ProSKU,
    p.Average,
    p.BirthDate,
    p.AverSalePmonth,
    p.WHcode,
    p.Sampling,
    p.FactoryCurrency,
    p.FOB_CIF,
    p.FOBCIFCost,
    p.ShelfLifeOfProduct,
    p.VOP,
    p.ProImage,
    p.Status,

    s.Id,
    s.ProId,
    s.CTNPerPallet,
    s.UOMCode,
    s.Width,
    s.Length,
    s.Height,
    s.CBMPerCTN,
    s.NetWeight,
    s.GrossWeight,
    s.CreatedDate,
    s.Status

FROM TPRProducts p
LEFT JOIN TblProductsScale s
    ON p.ProID = s.ProId
	inner join Category c
	on c.Id = p.ProCat";

        public const string GetAll = ListSelect + @"
ORDER BY p.ProID DESC;";

        public const string SearchByName = ListSelect + @"
WHERE p.ProName LIKE @Name
ORDER BY p.ProID DESC;";

        public const string SearchBySku = ListSelect + @"
WHERE p.ProSKU LIKE @Sku
ORDER BY p.ProID DESC;";

        public const string GetById = @"
SELECT
    p.ProID,
    p.ProNumY,
    p.ProNumS,
    p.ProNumYP,
    p.ProNumYC,
    p.Sup1,
    p.Sup2,
	c.Id AS categoryId,
    p.ProName,
    p.KhmerNameUnicode,

    p.ProDes,
    p.ProCat,
    c.CategoryName,
    p.ProPacksize,
    p.ProCurr,
    p.ProImpPri,
    p.ProRecLev,
    p.ProRecOrder,
    p.KhmerName,
    p.ProRem,
    p.Auto,
    p.ProfitAuto,
    p.ProTotQty,
    p.ProMadein,
    p.ProQtyPCase,
    p.ProQtyPPack,
    p.ProPckPri,
    p.ProPckDis,
    p.ProPckAllDis,
    p.ProRecomLev,
    p.Promotion,
    p.FormDLanded,
    p.ProUPriBY,
    p.ProAllowDisW,
    p.ProAllowDisU,
    p.ProDis,
    p.ExciseTax,
    p.PublicLightingTax,
    p.ProVAT,
    p.ProFinBuyin,
    p.ProUPrSE,
    p.ProProPer,
    p.ProUPriSeH,
    p.ProHolesaleper,
    p.ProHoleSalePP,
    p.ProRecPer,
    p.ProSKU,
    p.Average,
    p.BirthDate,
    p.AverSalePmonth,
    p.WHcode,
    p.Sampling,
    p.FactoryCurrency,
    p.FOB_CIF,
    p.FOBCIFCost,
    p.ShelfLifeOfProduct,
    p.VOP,
    p.ProImage,
    p.Status,

    s.Id,
    s.ProId,
    s.CTNPerPallet,
    s.UOMCode,
    s.Width,
    s.Length,
    s.Height,
    s.CBMPerCTN,
    s.NetWeight,
    s.GrossWeight,
    s.CreatedDate,
    s.Status

FROM TPRProducts p
LEFT JOIN TblProductsScale s
    ON p.ProID = s.ProId
	inner join Category c
	on c.Id = p.ProCat
WHERE p.ProID = @Id
ORDER BY p.ProID DESC;

";

        public const string ExistsByNameOtherId = @"SELECT COUNT(*)
              FROM TPRProducts
              WHERE ProNumY = @ProNumY
                AND ProID <> @ProID";

        public const string Update = @"
UPDATE TPRProducts
SET
    ProNumY = @ProNumY,
    ProNumS = @ProNumS,
    ProNumYP = @ProNumYP,
    ProNumYC = @ProNumYC,
    Sup1 = @Sup1,
    Sup2 = @Sup2,
    ProName = @ProName,
    KhmerNameUnicode = @KhmerNameUnicode,
    KhmerName = @KhmerName,

    ProDes = @ProDes,
    ProCat = @ProCat,
    ProPacksize = @ProPacksize,
    ProCurr = @ProCurr,
    ProImpPri = @ProImpPri,
    ProRecLev = @ProRecLev,
    ProRecOrder = @ProRecOrder,
    ProRem = @ProRem,
    Auto = @Auto,
    ProfitAuto = @ProfitAuto,
    ProTotQty = @ProTotQty,
    ProMadein = @ProMadein,
    ProQtyPCase = @ProQtyPCase,
    ProQtyPPack = @ProQtyPPack,
    ProPckPri = @ProPckPri,
    ProPckDis = @ProPckDis,
    ProPckAllDis = @ProPckAllDis,
    ProRecomLev = @ProRecomLev,
    Promotion = @Promotion,
    FormDLanded = @FormDLanded,
    ProUPriBY = @ProUPriBY,
    ProAllowDisW = @ProAllowDisW,
    ProAllowDisU = @ProAllowDisU,
    ProDis = @ProDis,
    ExciseTax = @ExciseTax,
    PublicLightingTax = @PublicLightingTax,
    ProVAT = @ProVAT,
    ProFinBuyin = @ProFinBuyin,
    ProUPrSE = @ProUPrSE,
    ProProPer = @ProProPer,
    ProUPriSeH = @ProUPriSeH,
    ProHolesaleper = @ProHolesaleper,
    ProHoleSalePP = @ProHoleSalePP,
    ProRecPer = @ProRecPer,
    ProSKU = @ProSKU,
    Average = @Average,
    BirthDate = @BirthDate,
    AverSalePmonth = @AverSalePmonth,
    WHcode = @WHcode,
    Sampling = @Sampling,
    FactoryCurrency = @FactoryCurrency,
    FOB_CIF = @FOB_CIF,
    FOBCIFCost = @FOBCIFCost,
    ShelfLifeOfProduct = @ShelfLifeOfProduct,
    VOP = @VOP,
    ProImage = @ProImage
WHERE ProID = @ProID;";

        public const string CheckScale = @"SELECT COUNT(*)
                  FROM TblProductsScale
                  WHERE ProId = @ProId";

        public const string UpdateScale = @"
UPDATE TblProductsScale
SET
    CTNPerPallet = @CTNPerPallet,
    UOMCode = @UOMCode,
    Width = @Width,
    Length = @Length,
    Height = @Height,
    CBMPerCTN = @CBMPerCTN,
    NetWeight = @NetWeight,
    GrossWeight = @GrossWeight,
    CreatedDate = @CreatedDate,
    Status = @Status
WHERE ProId = @ProId;";

        public const string InsertScaleUpdate = @"
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
    Status
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
    @CreatedDate,
    @Status
);";

        public const string Delete = @"
    DELETE FROM [DBJuJuBi].[dbo].[TPRProducts]
    WHERE ProID = @Id";

        public const string Exists = @"
    SELECT COUNT(1)
    FROM [DBJuJuBi].[dbo].[TPRProducts]
    WHERE ProID = @Id";

        public const string GetForPos = @"
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

ORDER BY p.ProductName;";

        // Stored procedure for exact-barcode lookup.
        public const string GetByBarcodeProc = "sp_GetProductByBarcode";

        public const string GetStatusById = "SELECT Status FROM TPRProducts WHERE ProID = @Id";

        public const string UpdateCaseNumber = @"
UPDATE TPRProducts
SET ProNumYC = @CaseNumber
WHERE ProID = @Id;";

        public const string UpdateBarcode = @"
UPDATE TPRProducts
SET ProNumY = @Barcode
WHERE ProID = @Id;";

        public const string UpdateOldBarcode = @"
UPDATE TPRProducts
SET OldProNumY = @OldBarcode
WHERE ProID = @Id;";

        public const string UpdatePackNumber = @"
UPDATE TPRProducts
SET ProNumYP = @PackNumber
WHERE ProID = @Id;";

        public const string UpdateStatus = @"
UPDATE TPRProducts
SET Status = @Status
WHERE ProID = @Id;";
    }
}
