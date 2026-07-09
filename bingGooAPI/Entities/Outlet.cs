namespace bingGooAPI.Entities
{
    public class Outlet : BaseEntity
    {
        public string OutletCode { get; set; } = null!;
        public string OutletName { get; set; } = null!;

        public string? Province { get; set; }
        public string? Phone { get; set; }
        public string? Manager { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }


        public string? VatNumber { get; set; }


        public string? PhotoUrl { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool HeadOffice { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        // បើក IsActive ឡើងវិញ ប្រសិនបើអ្នកត្រូវការប្រើវា
        public bool IsActive { get; set; } = true;
        public DateTime GrandOpeningDate { get; set; }
        public List<OutletPhoto> Images { get; set; } = new List<OutletPhoto>();
        public List<CitizenshipPhoto> CitizenshipPhotos { get; set; } = new List<CitizenshipPhoto>();
    }
}