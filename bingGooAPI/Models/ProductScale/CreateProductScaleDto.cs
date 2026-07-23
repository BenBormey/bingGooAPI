namespace JuJuBiAPI.Models.ProductScale
{
    public class CreateProductScaleDto
    {
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
    }
}
