namespace JuJuBiAPI.Entities
{
    public class ProductsScale
    {
    
            public decimal Id { get; set; }

            public decimal ProId { get; set; }

            public double? CTNPerPallet { get; set; }

        public string? UOMCode { get; set; }
        public string? UOMName { get; set; }

        public double? Width { get; set; }

            public double? Length { get; set; }

            public double? Height { get; set; }

            public double? CBMPerCTN { get; set; }

            public double? NetWeight { get; set; }

            public double? GrossWeight { get; set; }

            public bool Status { get; set; }
        public string ProNumY { get; set; }
        public DateTime CreatedDate { get; set; }
        }
    }
