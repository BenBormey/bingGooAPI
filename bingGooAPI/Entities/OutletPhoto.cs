using System.Text.Json.Serialization;

namespace JuJuBiAPI.Entities
{
    public class OutletPhoto
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string PhotoPath { get; set; } = null!;


        [JsonIgnore] 
        public Outlet? Outlet { get; set; }
    }
}
