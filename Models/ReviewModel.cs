using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ProductModel Product { get; set; } // Navigation property to Product
    }
}
