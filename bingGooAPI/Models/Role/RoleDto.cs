namespace bingGooAPI.Models.Role
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string RoleCode { get; set; } = default!;

        public string RoleName { get; set; } = default!;

        public string? Description { get; set; }

        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }
    }

}
