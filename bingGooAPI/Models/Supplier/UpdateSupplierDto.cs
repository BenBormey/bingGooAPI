using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.Supplier
{
    public class UpdateSupplierDto
    {
        public int SupplierID { get; set; }

        [Required]
        public string SupplierCode { get; set; } = null!;

        [Required]
        public string SupplierName { get; set; } = null!;

        public string? ContactName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? TaxNumber { get; set; }

        public string? KhmerSupAddress { get; set; }

        public string? Country { get; set; }

        public string? FaxLine2 { get; set; }
        public string? SupplierNamekh { get; set; } 
        public string? Website { get; set; }

        public string? LEAOTime { get; set; }

        public string? Note { get; set; }

        public string? ChequeName { get; set; }

        public int? Term { get; set; }

        public int? DayOrder { get; set; }

        public string? CountryOfPurchase { get; set; }

        public decimal? SetPercentOrderLevel { get; set; }

        public decimal? VATTEMP { get; set; }

        public bool Status { get; set; }
        public int TermId { get; set; }
    }
}