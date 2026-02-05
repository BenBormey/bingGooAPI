namespace bingGooAPI.Models
{
    public class LoginResponse
    {
        public string access_token { get; set; }

        public UserInfo user { get; set; }
    }
}
