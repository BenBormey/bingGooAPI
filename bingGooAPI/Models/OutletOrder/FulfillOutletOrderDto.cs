using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.OutletOrder
{
    public class FulfillOutletOrderDto
    {
        [Required]
        [MinLength(1)]
        public List<FulfillOutletOrderItemDto> Items { get; set; } = new();
    }

    public class FulfillOutletOrderItemDto
    {
        [Required]
        public int OutletOrderItemID { get; set; }

        [Range(1, int.MaxValue)]
        public int FulfilledQty { get; set; }
    }
}
