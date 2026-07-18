using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.PurchaseOrder
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        public int SupplierID { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public string? Note { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderItemDto
    {
        [Required]
        public string ProNumY { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal TaxPercent { get; set; }
    }
}
