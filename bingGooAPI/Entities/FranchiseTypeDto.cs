namespace JuJuBiAPI.Entities
{
    public class FranchiseTypeDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
