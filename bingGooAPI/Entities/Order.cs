using System;
using System.Collections.Generic;

namespace JuJuBiAPI.Entities
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

        public string InvoiceNo { get; set; }

        public string PaymentMethod { get; set; }

        public int? ShiftId { get; set; }

        public string VoidReason { get; set; }

        // Joined for display in the POS order list.
        public string CashierName { get; set; }

        public string OutletName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // ✅ Navigation Property
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
