namespace JuJuBiAPI.Entities
{
    public class OutletOrderItem
    {
        public int OutletOrderItemID { get; set; }

        public int OutletOrderID { get; set; }

        public string ProNumY { get; set; } = string.Empty;

        public int RequestedQty { get; set; }

        public int FulfilledQty { get; set; }

        public DateTime CreatedAt { get; set; }

        public OutletOrder? OutletOrder { get; set; }
    }
}
