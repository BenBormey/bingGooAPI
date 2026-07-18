namespace JuJuBiAPI.Entities
{
    public class ProductDeliveryLogistic
    {
        public int Id { get; set; }

        public string ProNumY { get; set; } = string.Empty;

        public int ProvinceId { get; set; }

        public string? ProvinceNameEN { get; set; }

        public decimal AdditionalCost { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
