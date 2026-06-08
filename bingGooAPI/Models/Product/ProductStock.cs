namespace bingGooAPI.Models.Product
{
    public class ProductStock
    {
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public decimal StockQty { get; set; }
        public DateTime LastUpdated { get; set; }
        public int OutletId { get; set; }
    }
}
