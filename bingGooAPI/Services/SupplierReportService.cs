using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Report;
using Dapper;
using System.Data;

namespace JuJuBiAPI.Services
{
    public class SupplierReportService : ISupplierReportRepository
    {
        private readonly IDbConnection _connection;

        public SupplierReportService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<SupplierReport>> GetReportAsync(SupplierReportFilter filter)
        {
            var sql = @"
SELECT
    p.BirthDate,
    p.ProNumY AS UnitNumber,
    CAST(p.ProQtyPPack AS NVARCHAR(50)) AS PackNumber,
    CAST(p.ProQtyPCase AS NVARCHAR(50)) AS CaseNumber,
    p.ProNumS AS MaterialCode,
    p.ProName AS ProductName,
    p.KhmerNameUnicode AS KhmerName,
    p.ProPacksize AS Size,
    p.FactoryCurrency,
    p.FOB_CIF AS FOBCIF,
    p.FOBCIFCost AS FactoryCost,
    p.ProCat AS Category,
    p.ProCurr AS Currency,
    p.ShelfLifeOfProduct AS ShelfLife,
    p.ProUPriBY AS Buyin,
    p.ProDis AS DiscountPercent,
    p.ExciseTax AS ExciseTaxPercent,
    p.PublicLightingTax AS PublicLightingPercent,
    p.ProVAT AS VATPercent,
    p.ProFinBuyin AS TotalBuyinPerCTN,
    '' AS Unit
FROM TPRProducts p
INNER JOIN Suppliers s
    ON s.SupplierCode = p.Sup1
WHERE
    (@Search IS NULL OR s.SupplierCode = @Search)
    AND (@Status IS NULL OR s.Status = @Status)
    AND (@Country IS NULL OR s.Country = @Country)
    AND (@FromDate IS NULL OR CAST(p.BirthDate AS DATE) >= @FromDate)
    AND (@ToDate IS NULL OR CAST(p.BirthDate AS DATE) <= @ToDate)
ORDER BY p.ProName;";

            return await _connection.QueryAsync<SupplierReport>(sql, filter);
        }
    }
}