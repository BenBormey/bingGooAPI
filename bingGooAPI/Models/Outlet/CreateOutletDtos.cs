using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Outlet
{
    public class CreateOutletDtos
    {
        [Required(ErrorMessage = "Outlet Code is required")]
        [MaxLength(50)]
        public string OutletCode { get; set; } = null!;

        [Required(ErrorMessage = "Outlet Name is required")]
        [MaxLength(150)]
        public string OutletName { get; set; } = null!;

        [MaxLength(100)]
        public string? Province { get; set; }

        public int? ProvinceId { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Manager { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(100)]
        public string? Email { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [MaxLength(255)]
        public string? PhotoPath { get; set; } 

        [MaxLength(50)]
        public string? VATNumber { get; set; }

        public bool HeadOffice { get; set; } = false;

        [Required]
        public int CreatedBy { get; set; }

  
        public List<string> PhotoPaths { get; set; } = new List<string>();
    }
}