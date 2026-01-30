namespace bingGooAPI.Helpers
{
    public class AppSettings
    {
        public string? Site { get; set; }
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Version { get; set; }
        public int ExpiryInMinutes { get; set; }
    }
}
