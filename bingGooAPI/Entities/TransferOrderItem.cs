namespace JuJuBiAPI.Entities
{
    public class TransferOrderItem
    {
        public int TransferOrderItemId { get; set; }

        public int TransferOrderId { get; set; }

        public string ProNumY { get; set; } = string.Empty;

        public int Qty { get; set; }

        public int ReceivedQty { get; set; }

        public decimal UnitCost { get; set; }

        public string? Remark { get; set; }

        public TransferOrder? TransferOrder { get; set; }
    }
}
