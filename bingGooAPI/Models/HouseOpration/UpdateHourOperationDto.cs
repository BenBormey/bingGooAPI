namespace JuJuBiAPI.Models.HouseOpration
{
    public class UpdateHourOperationDto
    {
        public int Id { get; set; }



        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool Is24Hours { get; set; }

        public bool Status { get; set; }
    }
}
