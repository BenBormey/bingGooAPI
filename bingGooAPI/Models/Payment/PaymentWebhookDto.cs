namespace JuJuBiAPI.Models.Payment
{
    public class PaymentWebhookDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }
}
