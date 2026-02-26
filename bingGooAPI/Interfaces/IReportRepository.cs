using bingGooAPI.Models.Report;

namespace bingGooAPI.Interfaces
{
    public interface IReportRepository
    {

        Task<List<PnLDto>> GetPnLAsync(DateTime from, DateTime to);


        Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate);

        //Task<CashFlowDto> GetCashFlowAsync(DateTime from, DateTime to);


        //Task<List<SalesSummaryDto>> GetSalesSummaryAsync(DateTime from, DateTime to);


        //Task<List<InventoryReportDto>> GetInventoryReportAsync(DateTime asOfDate);
        Task<List<SalesReportDto>> GetSalesReportAsync(DateTime from, DateTime to,int? outletid);

    }
}
