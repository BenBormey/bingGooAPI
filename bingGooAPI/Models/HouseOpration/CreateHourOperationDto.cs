namespace bingGooAPI.Models.HouseOpration
{
    public class CreateHourOperationDto
    {
     

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool Is24Hours { get; set; }

        public bool Status { get; set; }
    }
}
