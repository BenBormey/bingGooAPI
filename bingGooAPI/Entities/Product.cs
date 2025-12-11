namespace bingGooAPI.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string Brand { get; set; }
        public string Size { get; set; }

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal VatPercent { get; set; }

        public int Qty { get; set; }
        public string Remark { get; set; }
    }
}
