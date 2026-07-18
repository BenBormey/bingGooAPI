namespace JuJuBiAPI.Entities
{
    public class OutletOrder
    {
        public int OutletOrderID { get; set; }

        public string? OutletOrderNo { get; set; }

        public int OutletID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDate { get; set; }

        public string Status { get; set; } = "Draft";

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<OutletOrderItem> OutletOrderItems { get; set; } = new List<OutletOrderItem>();
    }
}
