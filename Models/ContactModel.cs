using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApplicationlaptop.Models
{
	public class ContactModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(100)]
		public string Subject { get; set; }

		[Required]
		[StringLength(1000)]
		public string Message { get; set; }

		public DateTime Ngaygui {  get; set; } 
	}
}
