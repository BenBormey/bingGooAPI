using JuJuBiAPI.Models.Report;

namespace JuJuBiAPI.Interfaces
{
    public interface ISupplierReportRepository
    {
        Task<IEnumerable<SupplierReport>> GetReportAsync(SupplierReportFilter filter);
    }
}