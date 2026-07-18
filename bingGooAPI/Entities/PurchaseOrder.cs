using System;
using System.Collections.Generic;

namespace JuJuBiAPI.Entities
{
    public class PurchaseOrder
    {
        public int PurchaseOrderID { get; set; }

        public string? PurchaseOrderNo { get; set; }

        public int SupplierID { get; set; }

        public int OutletID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public string Status { get; set; } = "Draft";

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}
