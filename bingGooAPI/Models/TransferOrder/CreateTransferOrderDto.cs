using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.TransferOrder
{
    public class CreateTransferOrderDto
    {
        [Required]
        public int FromOutletId { get; set; }

        [Required]
        public int ToOutletId { get; set; }

        public string? Remark { get; set; }

        public string? CreatedBy { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateTransferOrderItemDto> Items { get; set; } = new();
    }

    public class CreateTransferOrderItemDto
    {
        [Required]
        public string ProNumY { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Qty { get; set; }

        public decimal UnitCost { get; set; }

        public string? Remark { get; set; }
    }
}
