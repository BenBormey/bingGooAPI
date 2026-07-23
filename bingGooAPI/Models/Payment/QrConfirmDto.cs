namespace JuJuBiAPI.Models.Payment
{
    public class QrConfirmDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
    }
}
