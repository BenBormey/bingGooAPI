namespace JuJuBiAPI.Entities
{
    public class CitizenshipPhoto
    {
        public int Id { get; set; }

        public int OutletId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
