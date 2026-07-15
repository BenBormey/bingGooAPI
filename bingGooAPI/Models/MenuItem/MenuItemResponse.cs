namespace JuJuBiAPI.Models.MenuItem
{
    public sealed class MenuItemResponse
    {
        public int MenuItemId { get; set; }

        public int OutletId { get; set; }

        public string ProNumY { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;

        public decimal SellingPrice { get; set; }
        public string ProName { get; set; }
        public bool IsPromotion { get; set; }

        public decimal? Discount { get; set; }

        public decimal? PromotionPrice { get; set; }

        public DateTime? PromoStartDate { get; set; }

        public DateTime? PromoEndDate { get; set; }

        public bool IsActive { get; set; }

        public string? ImageFileName { get; set; }

        public string? Remark { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
