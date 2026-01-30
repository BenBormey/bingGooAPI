namespace bingGooAPI.Models.Role
{
    public class CreateRoleDto
    {
        public string RoleCode { get; set; } = default!;
        public string RoleName { get; set; }
        = default!;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; } = false; 
        public bool IsActive { get; set; } = true;  


    }
}
