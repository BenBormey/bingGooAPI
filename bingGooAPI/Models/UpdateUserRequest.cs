namespace bingGooAPI.Models
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; } = default!;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
    }
}
