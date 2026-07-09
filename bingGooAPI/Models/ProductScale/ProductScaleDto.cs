namespace bingGooAPI.Models.ProductScale
{
    public class ProductScaleDto
    {
        public int Id { get; set; }
        public decimal? CTNPerPallet { get; set; }

        public string? UOMCode { get; set; }

        public decimal? Width { get; set; }

        public decimal? Length { get; set; }

        public decimal? Height { get; set; }

        public decimal? CBMPerCTN { get; set; }

        public decimal? NetWeight { get; set; }

        public decimal? GrossWeight { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool Status { get; set; }
    }
}
