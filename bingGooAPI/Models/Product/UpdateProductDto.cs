namespace bingGooAPI.Models.Product
{
    namespace bingGooAPI.Models.Product
    {
        public class UpdateProductDto
        {
            public int ProductID { get; set; }
            public string ProductCode { get; set; }

            public string ProductName { get; set; }

            public int BrandId { get; set; }

            public int CategoryId { get; set; }

            public int SupplierId { get; set; }

            public string? ImageUrl { get; set; }

            public decimal CostPrice { get; set; }

            public decimal SellingPrice { get; set; }

            public decimal DiscountPercent { get; set; }

            public decimal DiscountAmount { get; set; }

            public decimal TaxPercent { get; set; }

            public bool Status { get; set; }
        }
    }

}
