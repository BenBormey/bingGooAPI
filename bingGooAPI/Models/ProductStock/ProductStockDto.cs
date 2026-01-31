namespace bingGooAPI.Models.ProductStock
{
    public class ProductStockDto
    {
        public int StockID { get; set; }

        public int ProductID { get; set; }

        public int BranchId { get; set; }

        public int StockQty { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
