namespace bingGooAPI.Models.Report
{
    /// <summary>
    /// Supplier Product Report
    /// </summary>
    public class SupplierReport
    {
        public DateTime? BirthDate { get; set; }

        public string? UnitNumber { get; set; }

        public string? PackNumber { get; set; }

        public string? CaseNumber { get; set; }

        public string? MaterialCode { get; set; }

        public string? ProductName { get; set; }

        public string? KhmerName { get; set; }

        public string? Size { get; set; }

        public string? FactoryCurrency { get; set; }

        public string? FOBCIF { get; set; }

        public decimal FactoryCost { get; set; }

        public string? Category { get; set; }

        public string? Currency { get; set; }

        public string? ShelfLife { get; set; }

        public decimal Buyin { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal ExciseTaxPercent { get; set; }

        public decimal PublicLightingPercent { get; set; }

        public decimal VATPercent { get; set; }

        public decimal TotalBuyinPerCTN { get; set; }

        public string? Unit { get; set; }
    }
}