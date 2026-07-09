using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Outlet
{
    public class CreateOutletCodeDto
    {
        [Required]
        [MaxLength(50)]
        public string OutletCode { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateOutletCodeDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OutletCode { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
