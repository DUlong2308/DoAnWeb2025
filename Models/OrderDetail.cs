using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationlaptop.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string ProductName { get; set; }

        [Required]
        public string OrderCode { get; set; }

        [Required]
        public int ProductId { get; set; } // Change from long to int

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int  OrderModelId { get; set; }

        //[ForeignKey("ProductId")]
        //public ProductModel Product { get; set; }

       
    }
}
