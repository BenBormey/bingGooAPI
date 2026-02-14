namespace bingGooAPI.Entities
{
    public class Payment
    {
        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        public string PaymentMethod { get; set; }  // Cash / QR / ABA

        public decimal AmountPaid { get; set; }

        public string PaymentStatus { get; set; }  // Paid / Pending / Failed

        public string? TransactionNo { get; set; }

        public DateTime PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal CashReceived { get; set; }
    }
}
