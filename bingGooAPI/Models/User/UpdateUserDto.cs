namespace bingGooAPI.Models.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = default!;

        public string? FullName { get; set; }
        public string? FullNameKh { get; set; }

        public int RoleId { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? AddressKh { get; set; }

        public bool IsActive { get; set; }

        public int OutletId { get; set; }
    }
}
