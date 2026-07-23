namespace JuJuBiAPI.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;

        public string KhmerCategoryName { get; set; } = string.Empty;



    }
}
