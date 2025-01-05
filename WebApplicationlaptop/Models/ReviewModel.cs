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

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } // New Rating property

        public ProductModel Product { get; set; } // Navigation property to Product

    }
}