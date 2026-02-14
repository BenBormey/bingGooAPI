using System;
using System.Collections.Generic;

namespace bingGooAPI.Entities
{
    public class Order
    {
        public int OrderID { get; set; }

        public int CartID { get; set; }

        public int UserID { get; set; }

        public int OutletID { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public string OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // ✅ Navigation Property
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
