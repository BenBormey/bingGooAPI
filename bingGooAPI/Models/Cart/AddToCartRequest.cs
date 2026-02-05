namespace bingGooAPI.Models.Cart
{
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int OutletId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountPercent { get; set; }
        public decimal TaxPercent { get; set; }
    }
}
