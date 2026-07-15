using System;

namespace JuJuBiAPI.Entities
{
    public class OutletProduct
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string ProNumY { get; set; } = string.Empty; 
        public bool CanSell { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string? Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}