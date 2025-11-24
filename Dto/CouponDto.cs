using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Dto
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
