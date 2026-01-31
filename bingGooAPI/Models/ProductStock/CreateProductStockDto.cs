namespace bingGooAPI.Models.ProductStock
{
    public class CreateProductStockDto
    {
        public int ProductID { get; set; }

        public int BranchId { get; set; }

        public int StockQty { get; set; }
    }
}
