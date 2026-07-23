using System.Data;
using Dapper;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Models.Report;
using JuJuBiAPI.Queries;

namespace JuJuBiAPI.Repositories
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


            var cash = await _connection.ExecuteScalarAsync<decimal>(
                ReportQueries.BalanceSheetCash, new { AsOfDate = asOfDate });

            bs.Assets.Add(new BalanceSheetItemDto
            {
                Code = "CASH",
                Name = "Cash & Bank",
                Amount = cash
            });


            var inventory = await _connection.ExecuteScalarAsync<decimal>(ReportQueries.BalanceSheetInventory);

            bs.Assets.Add(new BalanceSheetItemDto
            {
                Code = "INV",
                Name = "Inventory",
                Amount = inventory
            });


            // TODO: no supplier-invoice/AP tracking exists yet, so Accounts
            // Payable cannot be computed — report 0 rather than a fake figure.
            bs.Liabilities.Add(new BalanceSheetItemDto
            {
                Code = "AP",
                Name = "Accounts Payable",
                Amount = 0
            });


            var retainedEarnings = await _connection.ExecuteScalarAsync<decimal>(
                ReportQueries.BalanceSheetRetainedEarnings, new { AsOfDate = asOfDate });

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
            var data = await _connection.QueryAsync<PnLDto>(ReportQueries.PnL, new
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
                ReportQueries.SalesReportProc,
                parameters,
                commandType: CommandType.StoredProcedure);

            return data.ToList();
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            using var multi = await _connection.QueryMultipleAsync(ReportQueries.Dashboard);

            // Result sets arrive in the order the batch selects them — see
            // ReportQueries.Dashboard's numbered comments.
            var dto = await multi.ReadSingleAsync<DashboardDto>();

            dto.SalesByDay = (await multi.ReadAsync<DashboardDatePoint>()).ToList();
            dto.SalesByOutlet = (await multi.ReadAsync<DashboardNameValue>()).ToList();
            dto.TopProducts = (await multi.ReadAsync<DashboardNameValue>()).ToList();
            dto.SalesByPayment = (await multi.ReadAsync<DashboardNameValue>()).ToList();
            dto.LowStock = (await multi.ReadAsync<DashboardLowStockRow>()).ToList();
            dto.PendingOutletOrders = await multi.ReadSingleAsync<int>();
            dto.RecentOutletOrders = (await multi.ReadAsync<DashboardOutletOrderRow>()).ToList();

            return dto;
        }
    }
}
