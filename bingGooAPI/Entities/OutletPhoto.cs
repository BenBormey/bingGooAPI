using System.Text.Json.Serialization;

namespace bingGooAPI.Entities
{
    public class OutletPhoto
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string PhotoPath { get; set; } = null!;

        // បន្ថែមនេះដើម្បីងាយស្រួលក្នុង EF Core
        [JsonIgnore] // ការពារកុំឱ្យវាបាញ់ទិន្នន័យវិលជុំ (Object Cycle)
        public Outlet? Outlet { get; set; }
    }
}
