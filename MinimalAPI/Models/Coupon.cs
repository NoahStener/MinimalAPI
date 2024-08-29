using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Models
{
    public class Coupon
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public int Percent {  get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdate {  get; set; }
        public bool IsActive { get; set; }
    }
}
