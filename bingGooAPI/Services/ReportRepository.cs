using System.Data;
using Dapper;
using bingGooAPI.Interfaces;
using bingGooAPI.Models.Report;

namespace bingGooAPI.Services
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnection _connection;

        public ReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate)
        {
            var bs = new BalanceSheetDto
            {
                AsOfDate = asOfDate
            };


            var cashSql = @"
        SELECT ISNULL(SUM(p.AmountPaid), 0)
        FROM Payments p
        JOIN Orders o ON p.OrderID = o.OrderID
        WHERE o.OrderStatus = 'Paid'
          AND o.CreatedAt <= @AsOfDate
    ";

            var cash = await _connection.ExecuteScalarAsync<decimal>(
                cashSql, new { AsOfDate = asOfDate });

            bs.Assets.Add(new BalanceSheetItemDto
            {
                Code = "CASH",
                Name = "Cash & Bank",
                Amount = cash
            });

       
            var inventorySql = @"
SELECT 
    ISNULL(SUM(ps.StockQty * pr.CostPrice), 0)
FROM ProductStocks ps
JOIN Products pr ON ps.ProductID = pr.ProductID

    ";

            var inventory = await _connection.ExecuteScalarAsync<decimal>(inventorySql);

            bs.Assets.Add(new BalanceSheetItemDto
            {
                Code = "INV",
                Name = "Inventory",
                Amount = inventory
            });


            bs.Liabilities.Add(new BalanceSheetItemDto
            {
                Code = "AP",
                Name = "Accounts Payable",
                Amount = 112120
            });

          
            var retainedSql = @"
        SELECT 
            ISNULL(SUM(p.AmountPaid),0)
          - ISNULL(SUM(oi.Quantity * pr.CostPrice),0)
        FROM Orders o
        JOIN Payments p ON o.OrderID = p.OrderID
        JOIN OrderItems oi ON o.OrderID = oi.OrderID
        JOIN Products pr ON oi.ProductID = pr.ProductID
        WHERE o.OrderStatus = 'Paid'
          AND o.CreatedAt <= @AsOfDate
    ";

            var retainedEarnings = await _connection.ExecuteScalarAsync<decimal>(
                retainedSql, new { AsOfDate = asOfDate });

            bs.Equity.Add(new BalanceSheetItemDto
            {
                Code = "RE",
                Name = "Retained Earnings",
                Amount = retainedEarnings
            });

        
            //if (!bs.IsBalanced)
            //    throw new Exception("Balance Sheet is not balanced");

            return bs;
        }

        public async Task<List<PnLDto>> GetPnLAsync(DateTime from, DateTime to)
        {
            var sql = @"
                SELECT
               

                    SUM(p.AmountPaid) AS TotalSales,

                    SUM(oi.Quantity * pr.CostPrice) AS TotalCost,

                    SUM(p.AmountPaid)
                      - SUM(oi.Quantity * pr.CostPrice) AS Profit

                FROM Orders o
                JOIN Payments p ON o.OrderID = p.OrderID
                JOIN OrderItems oi ON o.OrderID = oi.OrderID
                JOIN Products pr ON oi.ProductID = pr.ProductID

                WHERE 
                    o.OrderStatus = 'Paid'
                    AND o.CreatedAt BETWEEN @From AND @To



              
            ";

            var data = await _connection.QueryAsync<PnLDto>(sql, new
            {
                From = from,
                To = to
            });

            return data.ToList();
        }

        public async Task<List<SalesReportDto>> GetSalesReportAsync(
           DateTime from,
           DateTime to,
           int? outletId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@FromDate", from.Date);
            parameters.Add("@ToDate", to.Date);
            parameters.Add("@OutletId", outletId);

            var data = await _connection.QueryAsync<SalesReportDto>(
                "rptSaleReportByOutlet",
                parameters,
                commandType: CommandType.StoredProcedure);

            return data.ToList();
        }


    }
}
