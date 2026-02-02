namespace bingGooAPI.Entities
{
    public class ProductStock
    {
        public int Id { get; set; }   


        public int ProductID { get; set; }
        public int BranchId { get; set; }
        public int OutletId { get; set; }

        public int StockQty { get; set; }
        public int MinStock { get; set; }

        public DateTime LastUpdated { get; set; }


        public Product Product { get; set; }
        public Branch Branch { get; set; }
        public Outlet Outlet { get; set; }
    }
}
