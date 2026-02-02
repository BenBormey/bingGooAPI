namespace bingGooAPI.Models.ProductStock
{
    public class UpdateProductStockDto
    {
        public int StockID { get; set; }

        public int ProductID { get; set; }

        public int BranchId { get; set; }

        public int StockQty { get; set; }
        public int OutletId { get;set; }
    }
}
