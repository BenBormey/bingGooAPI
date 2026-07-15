namespace JuJuBiAPI.Models
{
  
    public class OutletProductSave
    {
        public int OutletId { get; set; }
        public string ProNumY { get; set; } = string.Empty;
        public bool CanSell { get; set; } = true;
        public decimal? SellPrice { get; set; }        
        public decimal? DiscountPercent { get; set; } 
        public decimal? DiscountAmount { get; set; }
        public string? Currency { get; set; }
        public bool IsActive { get; set; } = true;
    }

 
    public class SellableProduct
    {
        public string ProNumY { get; set; } = string.Empty;
        public int ProID { get; set; }
        public string? ProName { get; set; }
        public string? KhmerName { get; set; }
        public decimal SellPrice { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}