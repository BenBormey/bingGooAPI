namespace bingGooAPI.Entities
{
    public class Supplier : BaseEntity
    {
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
