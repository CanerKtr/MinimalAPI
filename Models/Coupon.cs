namespace MinimalAPI.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Percent { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
