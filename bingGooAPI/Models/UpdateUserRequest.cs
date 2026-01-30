namespace bingGooAPI.Models
{
    public class UpdateUserRequest
    {
        public string Username { get; set; } = default!;
        public string FullName { get; set; } = default!;      
        public string FullNameKh { get; set; } = default!;   

        public int RoleId { get; set; }       
        public int OutLetId { get; set; }    
        public bool IsActive { get; set; }
    }
}
