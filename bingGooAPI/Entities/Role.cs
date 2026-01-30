namespace bingGooAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string RoleCode { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
