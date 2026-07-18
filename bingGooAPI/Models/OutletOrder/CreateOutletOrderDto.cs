using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.OutletOrder
{
    public class CreateOutletOrderDto
    {
        [Required]
        public int OutletID { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public string? Note { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOutletOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOutletOrderItemDto
    {
        [Required]
        public string ProNumY { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int RequestedQty { get; set; }
    }
}
