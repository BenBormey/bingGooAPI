namespace bingGooAPI.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

        public string FullName { get; set; } = default!;      
        public string FullNameKh { get; set; } = default!;   

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string Phone { get; set; }
        public string address { get; set; }
        public string addressKh { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        public int outLetId { get; set; }   
        public string OutLetName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
