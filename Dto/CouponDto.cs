using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Dto
{
    public class CouponDto
    {
        [Required]
        public int Id { get; set; }

        public string? Name { get; set; }
        public int? Percent { get; set; }
        public bool? IsActive { get; set; }
    }
    public class CouponDtoWithoutId
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Percent { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
