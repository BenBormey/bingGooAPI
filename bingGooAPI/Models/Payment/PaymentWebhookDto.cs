namespace bingGooAPI.Models.Payment
{
    public class PaymentWebhookDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } 
        public string TransactionId { get; set; }
    }
}
