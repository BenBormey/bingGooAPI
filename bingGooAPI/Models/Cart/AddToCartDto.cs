namespace bingGooAPI.Models.Cart
{
    public class AddToCartDto
    {
        public int UserID { get; set; }
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal DiscountPercent { get; set; }
    }
}
