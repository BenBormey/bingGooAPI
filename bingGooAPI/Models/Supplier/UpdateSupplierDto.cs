using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Supplier
{
    public class UpdateSupplierDto
    {
      
        public int SupplierID { get; set; }


        public string SupplierCode { get; set; } = null!;

     
        public string SupplierName { get; set; } = null!;

        public string? ContactName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? TaxNumber { get; set; }

        public bool Status { get; set; }
    }
}
