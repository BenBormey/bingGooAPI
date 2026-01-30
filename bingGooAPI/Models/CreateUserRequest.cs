namespace bingGooAPI.Models
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string FullName { get; set; } = default!;
        public string FullNameKh { get; set; } = default!;
        public int OutletId { get; set; }
        public int RoleId { get; set; } = 2; 
    }
}
