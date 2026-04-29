namespace bingGooAPI.Models.Outlet
{
    public class OutletListDto
    {
        public int Id { get; set; }

        public string OutletCode { get; set; } = null!;
        public string OutletName { get; set; } = null!;

        public string? Province { get; set; }
        public string? Phone { get; set; }

        public string? Manager { get; set; }

        public bool HeadOffice { get; set; }
        public string PhotoPath { get;set; } = null!;
        public string VATNumber { get; set; }

        public int? ProvinceId { get; set; }

        public List<string> Photos { get; set; } = new List<string>();

        public bool IsActive { get; set; }
    }
}
