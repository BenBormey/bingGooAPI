namespace bingGooAPI.Models
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string FullName { get; set; } = default!;
        public string FullNameKh { get; set; } = default!;

        public int RoleId { get; set; } = 2;

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? AddressKh { get; set; }

        public int OutletId { get; set; }
    }
}
