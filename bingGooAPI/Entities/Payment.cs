namespace JuJuBiAPI.Entities
{
    public class Payment
    {
        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;  // Cash / QR / ABA

        public decimal AmountPaid { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;  // Paid / Pending / Failed

        public string? TransactionNo { get; set; }

        public DateTime PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal CashReceived { get; set; }
    }
}
