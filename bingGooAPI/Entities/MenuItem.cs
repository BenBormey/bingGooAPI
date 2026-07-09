namespace JuJuBis.Domain.Entities;

public sealed class MenuItem
{
    public int MenuItemId { get; set; }

    public int OutletId { get; set; }

    public string OutletName { get; set; } = string.Empty;

    public string ProNumY { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;


    public int CurrencyId { get; set; }


    public string CurrencyCode { get; set; } = string.Empty;

    public string? CurrencyName { get; set; }

    public decimal SellingPrice { get; set; }

    public bool IsPromotion { get; set; }

    public decimal? Discount { get; set; }

 
    public decimal? PromotionPrice { get; set; }

    public DateTime? PromoStartDate { get; set; }

    public DateTime? PromoEndDate { get; set; }

    public bool IsActive { get; set; }

    public string? ImageFileName { get; set; }

    public string? Remark { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}