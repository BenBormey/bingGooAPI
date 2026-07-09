using System.ComponentModel.DataAnnotations;

namespace bingGooAPI.Models.Term
{
    public class CreateTermDayDto
    {
        [Required]
        public int CountDay { get; set; }
    }

    public class UpdateTermDayDto
    {
        public int Id { get; set; }

        [Required]
        public int CountDay { get; set; }
    }
}
