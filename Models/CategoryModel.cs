using System.ComponentModel.DataAnnotations;

namespace WebApplicationlaptop.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required (ErrorMessage = "yeu cau nhap ten danh muc" )]
		public string Name { get; set; }
		[Required ( ErrorMessage = "yeu cau nhap mo ta danh muc ")]
		public string Description { get; set; }
		public string Slug { get; set; }
		public int Status { get; set; }

        public List<ProductModel> Products { get; set; }


    }
}
