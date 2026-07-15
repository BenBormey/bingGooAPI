namespace JuJuBiAPI.Entities
{
    public class HourOperation
    {
        public int Id { get; set; }



        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool Is24Hours { get; set; }

        public bool Status { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
