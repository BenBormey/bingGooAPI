namespace bingGooAPI.Models.Report
{
    public class SalesReportDto
    {
        public DateTime SaleDate { get; set; }

        public int TotalOrders { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalSales { get; set; }
    }
}
