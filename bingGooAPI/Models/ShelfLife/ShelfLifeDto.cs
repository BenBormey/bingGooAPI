using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.ShelfLife
{
    public class CreateShelfLifeDto
    {
        [MaxLength(150)]
        public string? ShelfLifeName { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int ShelfLifeValue { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShelfLifeUnit { get; set; } = null!;
    }

    public class UpdateShelfLifeDto
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string? ShelfLifeName { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int ShelfLifeValue { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShelfLifeUnit { get; set; } = null!;
    }
}
