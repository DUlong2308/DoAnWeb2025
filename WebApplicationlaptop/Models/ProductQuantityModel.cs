using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
    public class ProductQuantityModel
    {

         public int Id { get; set; }

        [Required(ErrorMessage = "yêu cầu nhập số lượng sản phẩm  ")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public DateTime DateCreated { get; set; } 


    }
}
