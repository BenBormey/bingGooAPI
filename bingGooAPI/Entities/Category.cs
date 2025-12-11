namespace bingGooAPI.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Remark { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
