using System;

namespace JuJuBiAPI.Models.OutletProduct
{

    public class OutletProductItem
    {
        public string ProNumY { get; set; } = string.Empty;   
        public int ProID { get; set; }
        public string? ProName { get; set; }
        public string? KhmerName { get; set; }
        public string? Category { get; set; }
        public string? Currency { get; set; }


        public decimal DefaultSellPrice { get; set; }
        public decimal DefaultDiscount { get; set; }

 
        public int? OutletProductId { get; set; }
        public bool CanSell { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public bool IsActive { get; set; }

        // What the POS should actually use (override, else default)
        public decimal EffectiveSellPrice { get; set; }
        public decimal EffectiveDiscount { get; set; }
    }
}