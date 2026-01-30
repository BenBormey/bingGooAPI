using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Outlet
{
    public class CreateOutletDtos
    {
        [Required]
        [MaxLength(50)]
        public string OutletCode { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string OutletName { get; set; } = null!;

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

        public bool HeadOffice { get; set; } = false;



    
        public int CreatedBy { get; set; }

    
    }
}
