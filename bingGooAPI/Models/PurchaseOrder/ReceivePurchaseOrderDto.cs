using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.PurchaseOrder
{
    public class ReceivePurchaseOrderDto
    {
        [Required]
        [MinLength(1)]
        public List<ReceivePurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class ReceivePurchaseOrderItemDto
    {
        [Required]
        public int PurchaseOrderItemID { get; set; }

        [Range(1, int.MaxValue)]
        public int ReceivedQty { get; set; }
    }
}
