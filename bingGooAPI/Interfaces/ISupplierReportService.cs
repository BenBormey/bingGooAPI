using bingGooAPI.Models.Report;

namespace bingGooAPI.Services.Report
{
    public interface ISupplierReportService
    {
        Task<IEnumerable<SupplierReport>> GetSupplierReportAsync(SupplierReportFilter filter);
    }
}