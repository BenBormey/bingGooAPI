using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Product;
using bingGooAPI.Models.ProductScale;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace bingGooAPI.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _connection;

        public ProductRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<CreateProductDto> CreateAsync(CreateProductDto product)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Check duplicate barcode across ALL barcode columns
                // (PK is ProNumY, but pack/case/SKU duplicates cause lookup bugs later)
                var exists = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(*)
              FROM [DBJuJuBi].[dbo].[TPRProducts]
              WHERE ProNumY = @ProNumY
                 OR (@ProNumYP <> '' AND ProNumYP = @ProNumYP)
                 OR (@ProNumYC <> '' AND ProNumYC = @ProNumYC)",
                    new
                    {
                        product.ProNumY,
                        ProNumYP = product.ProNumYP ?? "",
                        ProNumYC = product.ProNumYC ?? ""
                    },
                    transaction);

                if (exists > 0)
                    throw new Exception("Product Number (Unit/Pack/Case) already exists.");

                var sql = @"
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

                //    ISNULL(@ProAllowDisW, 0), ISNULL(@ProAllowDisU, 1),
                //    ProAllowDisW, ProAllowDisU,

                //
                var productId = await _connection.ExecuteScalarAsync<int>(
                    sql,
                    product,
                    transaction);

                // Insert Product Scale
                if (product.ProductScale != null)
                {
                    var scaleSql = @"
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

                    await _connection.ExecuteAsync(
                        scaleSql,
                        new
                        {
                            ProId = productId,
                            product.ProductScale.CTNPerPallet,
                            product.ProductScale.UOMCode,
                            product.ProductScale.Width,
                            product.ProductScale.Length,
                            product.ProductScale.Height,
                            product.ProductScale.CBMPerCTN,
                            product.ProductScale.NetWeight,
                            product.ProductScale.GrossWeight,
                            CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                            product.ProductScale.Status
                        },
                        transaction);
                }

                transaction.Commit();

                product.Id = productId;

                return product;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }

        public async Task<List<ProductListDto>> GetAllAsync()
        {
            var sql = @"
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
ORDER BY p.ProID DESC;";

            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                sql,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                splitOn: "Id");

            return result.ToList();
        }

        public async Task<ProductListDto?> GetByIdAsync(int id)
        {
            var sql = @"
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

            var result = await _connection.QueryAsync<ProductListDto, ProductScaleDto, ProductListDto>(
                sql,
                (product, scale) =>
                {
                    product.ProductScale = scale;
                    return product;
                },
                new { Id = id },
                splitOn: "Id");

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(UpdateProductDto product)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // Check duplicate Product Number
                var exists = await _connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(*)
              FROM TPRProducts
              WHERE ProNumY = @ProNumY
                AND ProID <> @ProID",
                    new { product.ProNumY, product.ProID },
                    transaction);

                if (exists > 0)
                    throw new Exception("Product Number already exists.");

                // Update Product
                var sql = @"
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

                var rows = await _connection.ExecuteAsync(
                    sql,
                    product,
                    transaction);

                // Update Product Scale
                if (product.ProductScale != null)
                {
                    var checkScale = await _connection.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(*)
                  FROM TblProductsScale
                  WHERE ProId = @ProId",
                        new { ProId = product.ProID },
                        transaction);

                    if (checkScale > 0)
                    {
                        var updateScale = @"
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

                        await _connection.ExecuteAsync(
                            updateScale,
                            new
                            {
                                ProId = product.ProID,
                                product.ProductScale.CTNPerPallet,
                                product.ProductScale.UOMCode,
                                product.ProductScale.Width,
                                product.ProductScale.Length,
                                product.ProductScale.Height,
                                product.ProductScale.CBMPerCTN,
                                product.ProductScale.NetWeight,
                                product.ProductScale.GrossWeight,
                                CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                                product.ProductScale.Status
                            },
                            transaction);
                    }
                    else
                    {
                        var insertScale = @"
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

                        await _connection.ExecuteAsync(
                            insertScale,
                            new
                            {
                                ProId = product.ProID,
                                product.ProductScale.CTNPerPallet,
                                product.ProductScale.UOMCode,
                                product.ProductScale.Width,
                                product.ProductScale.Length,
                                product.ProductScale.Height,
                                product.ProductScale.CBMPerCTN,
                                product.ProductScale.NetWeight,
                                product.ProductScale.GrossWeight,
                                CreatedDate = product.ProductScale.CreatedDate ?? DateTime.Now,
                                product.ProductScale.Status
                            },
                            transaction);
                    }
                }

                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = @"
    DELETE FROM [DBJuJuBi].[dbo].[TPRProducts]
    WHERE ProID = @Id";

            var rows = await _connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var sql = @"
    SELECT COUNT(1)
    FROM [DBJuJuBi].[dbo].[TPRProducts]
    WHERE ProID = @Id";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        public async Task<List<ProductPosDto>> GetForPosAsync(int outletId, int? categoryId)
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

ORDER BY p.ProductName;";

            var data = await _connection.QueryAsync<ProductPosDto>(
                sql,
                new { OutletId = outletId, CategoryId = categoryId });

            return data.ToList();
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            var product = (await _connection.QueryAsync<Product, ProductScaleDto, Product>(
                "sp_GetProductByBarcode",
                (p, scale) =>
                {
                    p.ProductScale = scale;
                    return p;
                },
                new
                {
                    Barcode = barcode
                },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id"
            )).FirstOrDefault();

            return product;
        }
    }
}