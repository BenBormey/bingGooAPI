namespace bingGooAPI.Models.Order
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }

        public int OutletId { get; set; }

        public int CartId { get; set; }

        // Optional (future)
        public string? Note { get; set; }

        // Payment Method (future use)
        public string? PaymentMethod { get; set; }
    }
}
