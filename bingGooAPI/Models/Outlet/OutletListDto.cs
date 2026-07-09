using bingGooAPI.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace bingGooAPI.Models.Outlet
{
    public class OutletListDto
    {
        public int Id { get; set; }

        public string OutletCode { get; set; } = null!;
        public string OutletName { get; set; } = null!;
       
        public string? Address { get; set; }
        public string? FrancisePhone { get; set; }
        public string ProvinceNameEN { get;set; } = null!;
        public string? Manager { get; set; }
        public int HourOperationId { get; set; }
        public string? HourOperation { get; set; }
        public string OutletPhone { get; set; } = null!;
        public int ProvinceId { get; set; }
        public bool HeadOffice { get; set; }
        public string PhotoPath { get;set; } = null!;
        public string VATNumber { get; set; }
        public int FranchiseId { get; set; }
        public string Position { get; set; } = null!;
        public DateTime GrandOpeningDate { get; set; }
        public string Email { get; set; }
        //public string Position { get; set; } = null!;

        //    public List<string> Photos { get; set; } = new List<string>();

        public List<Photo> Photos { get; set; } = new List<Photo>();
        public List<CitizenshipPhoto> CitizenshipPhotos { get; set; } = new List<CitizenshipPhoto>();
        public string? TypeName { get; set; }
        public bool Is24Hours { get; set; }
        public bool IsActive { get; set; }
    }
}
