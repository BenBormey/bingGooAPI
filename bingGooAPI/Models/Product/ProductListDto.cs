namespace bingGooAPI.Models.Product
{
    public class ProductListDto
    {
        public int ProductID { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public int BrandID { get; set; }
        public string BrandName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public string? ImageUrl { get; set; }

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxPercent { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        //public int StockQty { get; set; }
    }
}
