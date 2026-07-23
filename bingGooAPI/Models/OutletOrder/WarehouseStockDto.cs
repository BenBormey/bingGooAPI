namespace JuJuBiAPI.Models.OutletOrder
{
    // One row of the warehouse (HeadOffice outlet) stock — used by the
    // approval screen to show what's available to fulfill from.
    public class WarehouseStockDto
    {
        public string ProNumY { get; set; } = string.Empty;
        public decimal StockQty { get; set; }
    }
}
