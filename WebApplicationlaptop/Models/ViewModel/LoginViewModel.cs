using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password), Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}