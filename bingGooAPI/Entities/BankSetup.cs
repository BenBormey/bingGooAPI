namespace JuJuBiAPI.Entities
{
    public class BankSetup
    {
        public int BankId { get; set; }

        public string BankCode { get; set; } = string.Empty;

        public string BankName { get; set; } = string.Empty;

        public string BankType { get; set; } = string.Empty;

        public string MerchantId { get; set; } = string.Empty;

        public string MerchantName { get; set; } = string.Empty;

        public string ApiUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string ApiSecret { get; set; } = string.Empty;

        public string CallbackUrl { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }
    }
}
