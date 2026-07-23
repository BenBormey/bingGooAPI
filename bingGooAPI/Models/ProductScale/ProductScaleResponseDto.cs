namespace JuJuBiAPI.Models.ProductScale
{
    public class ProductScaleResponseDto
    {
        public decimal Id { get; set; }
        public decimal ProId { get; set; }

        public double? CTNPerPallet { get; set; }
        public string? UOM { get; set; }

        public double? Width { get; set; }
        public double? Length { get; set; }
        public double? Height { get; set; }

        public double? CBMPerCTN { get; set; }
        public double? NetWeight { get; set; }
        public double? GrossWeight { get; set; }
        public string ProNumY { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
