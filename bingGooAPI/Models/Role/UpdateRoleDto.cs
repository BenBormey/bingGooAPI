namespace bingGooAPI.Models.Role
{
    public class UpdateRoleDto
    {
        public string RoleCode { get; set; } = default!;

        public string RoleName { get; set; } = default!;

        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }
    }

}
