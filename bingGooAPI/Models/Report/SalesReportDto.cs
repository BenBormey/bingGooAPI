namespace bingGooAPI.Models.Report
{
    public class SalesReportDto
    {
        public int OutletId { get; set; }

        public string OutletName { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public int TotalQty { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal NetAmount { get; set; }

        public DateTime SaleDate { get; set; }
        public string InvoiceNo { get; set; }
    }
}
