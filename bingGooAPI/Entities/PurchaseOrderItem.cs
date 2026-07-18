using System;

namespace JuJuBiAPI.Entities
{
    public class PurchaseOrderItem
    {
        public int PurchaseOrderItemID { get; set; }

        public int PurchaseOrderID { get; set; }

        public string ProNumY { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxPercent { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TotalCost { get; set; }

        public int ReceivedQty { get; set; }

        public DateTime CreatedAt { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}
