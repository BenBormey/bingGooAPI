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



        public bool IsActive { get; set; }
    }
}
