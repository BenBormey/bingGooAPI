using System.ComponentModel.DataAnnotations;

namespace JuJuBiAPI.Models.ProductDeliveryLogistic
{
    public class CreateProductDeliveryLogisticDto
    {
        [Required]
        public string ProNumY { get; set; } = string.Empty;

        [Required]
        public int ProvinceId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AdditionalCost { get; set; }
    }
}
