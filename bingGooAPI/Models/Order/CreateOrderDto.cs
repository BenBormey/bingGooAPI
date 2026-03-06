namespace bingGooAPI.Models.Order
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }

        public int OutletId { get; set; }

        public int CartId { get; set; }


        public string? Note { get; set; }


        public string? PaymentMethod { get; set; }
    }
}
