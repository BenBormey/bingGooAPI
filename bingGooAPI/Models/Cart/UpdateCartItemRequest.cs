namespace bingGooAPI.Models.Cart
{
    public class UpdateCartItemRequest
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
