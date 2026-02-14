namespace bingGooAPI.Models.Payment
{
    public class CashPaymentDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public decimal CashReceived { get; set; }
    }
}
