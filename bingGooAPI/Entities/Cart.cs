namespace bingGooAPI.Entities
{
    public class Cart
    {
        public int CartID { get; set; }

        public int UserID { get; set; }
        public int? OutletID { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<CartItem> CartItems { get; set; }
    }
}
