using System.ComponentModel.DataAnnotations;

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
        public int ProductId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int OrderModelId { get; set; }

        public decimal ShippingCost { get; set; }


        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }


        //[ForeignKey("ProductId")]
        //public ProductModel Product { get; set; }
    }
}