namespace JuJuBiAPI.Entities
{
    public class TransferOrder
    {
        public int TransferOrderId { get; set; }

        public string? TransferNo { get; set; }

        public int FromOutletId { get; set; }

        public int ToOutletId { get; set; }

        public DateTime TransferDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Remark { get; set; }

        public string? CreatedBy { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? ReceivedBy { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<TransferOrderItem> TransferOrderItems { get; set; } = new List<TransferOrderItem>();
    }
}
