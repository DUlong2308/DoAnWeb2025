using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
	public class UserModel
	{
		public int Id {  get; set; }
		[Required]
		public string UserName { get; set; }
		[Required,EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password), Required]
		public string Password { get; set; }
		[Required(ErrorMessage = "Password confirmation is required")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; }

	}
}
