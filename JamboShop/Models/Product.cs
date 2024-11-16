using System.ComponentModel.DataAnnotations;

namespace JamboShop.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = "";

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal Stock { get; set; }
    }
}
