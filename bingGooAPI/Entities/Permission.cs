namespace JuJuBiAPI.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }
}
