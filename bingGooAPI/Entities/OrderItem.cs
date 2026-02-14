namespace bingGooAPI.Entities
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxPercent { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }


        public Order Order { get; set; }
    }
}
