using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.TransferOrder
{
    public class ReceiveTransferOrderDto
    {
        public string? ReceivedBy { get; set; }

        [Required]
        [MinLength(1)]
        public List<ReceiveTransferOrderItemDto> Items { get; set; } = new();
    }

    public class ReceiveTransferOrderItemDto
    {
        [Required]
        public int TransferOrderItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int ReceivedQty { get; set; }
    }
}
