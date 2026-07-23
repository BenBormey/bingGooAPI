namespace JuJuBiAPI.Models.Report
{
    // Everything the MD dashboard shows, fetched in one call
    // (GET api/Report/dashboard).
    public class DashboardDto
    {
        public decimal SalesToday { get; set; }
        public decimal SalesYesterday { get; set; }
        public decimal SalesWeek { get; set; }
        public decimal SalesMonth { get; set; }
        public int OrdersToday { get; set; }
        public decimal VatMonth { get; set; }
        public decimal StockValue { get; set; }
        public int PendingOutletOrders { get; set; }

        public List<DashboardDatePoint> SalesByDay { get; set; } = new();
        public List<DashboardNameValue> SalesByOutlet { get; set; } = new();
        public List<DashboardNameValue> TopProducts { get; set; } = new();
        public List<DashboardNameValue> SalesByPayment { get; set; } = new();
        public List<DashboardLowStockRow> LowStock { get; set; } = new();
        public List<DashboardOutletOrderRow> RecentOutletOrders { get; set; } = new();
    }

    public class DashboardOutletOrderRow
    {
        public string OutletOrderNo { get; set; } = string.Empty;
        public string Outlet { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public int Items { get; set; }
    }

    public class DashboardDatePoint
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }

    public class DashboardNameValue
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class DashboardLowStockRow
    {
        public string Product { get; set; } = string.Empty;
        public string Outlet { get; set; } = string.Empty;
        public decimal StockQty { get; set; }
    }
}
