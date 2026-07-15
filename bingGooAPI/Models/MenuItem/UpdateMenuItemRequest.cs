namespace JuJuBiAPI.Models.MenuItem
{
    public sealed class UpdateMenuItemRequest
    {
        public int MenuItemId { get; set; }

        public string CurrencyCode { get; set; } = string.Empty;

        public decimal SellingPrice { get; set; }

        public bool IsPromotion { get; set; }

        public decimal? Discount { get; set; }

        public decimal? PromotionPrice { get; set; }

        public DateTime? PromoStartDate { get; set; }

        public DateTime? PromoEndDate { get; set; }

        public bool IsActive { get; set; }

        public string? ImageFileName { get; set; }

        public string? Remark { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
