namespace bingGooAPI.Entities
{
    public class Outlet : BaseEntity
    {
        public string OutletCode { get; set; }
        public string OutletName { get; set; }
        public string Province { get; set; }
        public string Phone { get; set; }
        public string Manager { get; set; }
        public string Address { get; set; }

        public bool HeadOffice { get; set; }
    }
}
