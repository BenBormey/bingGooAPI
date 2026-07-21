namespace JuJuBiAPI.Entities
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

        // Per-item prep instruction entered on the POS (sugar/ice level, etc.).
        public string Note { get; set; }

        // Joined for display on the POS order detail panel / receipt reprint.
        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ImageUrl { get; set; }


        public Order Order { get; set; }
    }
}
