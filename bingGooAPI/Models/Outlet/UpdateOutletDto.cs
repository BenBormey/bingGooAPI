using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Outlet
{
    public class UpdateOutletDto
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(150)]
        public string OutletCode { get; set; }
        public string? OutletName { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Manager { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool? HeadOffice { get; set; }

        public int? ProvinceId { get; set; }

 
        public string PhotoPath { get; set; } = null!;
        public string VATNumber { get; set; }
        public List<string> PhotoPaths { get; set; } = new List<string>();


        public bool? IsActive { get; set; }
    }
}
