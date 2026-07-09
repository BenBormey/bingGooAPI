namespace bingGooAPI.Entities
{
    public class ShelfLifeEntity
    {
        public int Id { get; set; }
        public string? ShelfLifeName { get; set; }
        public bool IsActive { get; set; }
        public int ShelfLifeValue { get; set; }
        public string ShelfLifeUnit { get; set; } = null!;
    }
}
