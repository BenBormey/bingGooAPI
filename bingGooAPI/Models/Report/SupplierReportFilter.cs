namespace JuJuBiAPI.Models.Report
{
    /// <summary>
    /// Query parameters for the supplier report. All optional -
    /// null means "no filter on this field".
    /// </summary>
    public class SupplierReportFilter
    {
        /// <summary>Matches SupplierName, SupplierCode or ContactName.</summary>
        public string? Search { get; set; }

        /// <summary>Active (true) / Inactive (false) supplier status.</summary>
        public bool? Status { get; set; }

        /// <summary>Filter by supplier country.</summary>
        public string? Country { get; set; }

        /// <summary>Supplier created on/after this date.</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>Supplier created on/before this date.</summary>
        public DateTime? ToDate { get; set; }
    }
}