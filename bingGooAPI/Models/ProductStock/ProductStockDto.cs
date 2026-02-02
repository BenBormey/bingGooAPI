namespace bingGooAPI.Models.ProductStock
{
    public class ProductStockDto
    {
        public int StockID { get; set; }

        public int ProductID { get; set; }

        public int BranchId { get; set; }

        public int StockQty { get; set; }

        public DateTime LastUpdated { get; set; }
        public int OutletId { get; set; }   
        public string OutletName { get; set; }
        public string BranchName { get; set; }
       public string ProductName { get; set; }
    }
}
