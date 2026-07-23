namespace JuJuBiAPI.Models
{
    public class LoginResponse
    {
        public string access_token { get; set; } = string.Empty;

        public UserInfo user { get; set; } = null!;
    }
}
