using bingGooAPI.Interfaces;
using System;
using System.Collections.Generic;

namespace bingGooAPI.Entities
{
    public class Product
    {
        
        public int ProductID { get; set; }


        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public int BrandID { get; set; }
        public Branch Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

     
        public string? ImageUrl { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxPercent { get; set; }

        public bool Status { get; set; } = true;

      
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductStock>? ProductStocks { get; set; }
    }
}
