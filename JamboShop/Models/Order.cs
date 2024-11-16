using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JamboShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "";

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
