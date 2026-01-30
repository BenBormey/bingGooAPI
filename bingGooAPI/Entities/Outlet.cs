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

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool HeadOffice { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

      //  public bool IsActive { get; set; } = true;
    }
}
