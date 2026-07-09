using bingGooAPI.Models.Report;

namespace bingGooAPI.Interfaces
{
    public interface ISupplierReportRepository
    {
        Task<IEnumerable<SupplierReport>> GetReportAsync(SupplierReportFilter filter);
    }
}