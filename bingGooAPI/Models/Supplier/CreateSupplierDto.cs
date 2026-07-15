namespace JuJuBiAPI.Models.Supplier
{
    public class CreateSupplierDto
    {
        public string? SupplierCode { get; set; }

        public string? SupplierName { get; set; }

        public string? ContactName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? SupplierNamekh { get;set; }

        public string? TaxNumber { get; set; }

        public string? KhmerSupAddress { get; set; }

        public string? Country { get; set; }

        public string? FaxLine2 { get; set; }

        public string? Website { get; set; }

        public string? LEAOTime { get; set; }

        public string? Note { get; set; }

        public string? ChequeName { get; set; }

        public int? Term { get; set; }

        public int? DayOrder { get; set; }

        public string? CountryOfPurchase { get; set; }

        public decimal? SetPercentOrderLevel { get; set; }

        public decimal? VATTEMP { get; set; }

        public bool Status { get; set; } = true;
        public int TermId { get; set; }
    }
}