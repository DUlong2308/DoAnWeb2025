using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Responsibilty
{
	public class SeedData
	{
		public static void SeedingData (DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel Macbook = new CategoryModel { Name = "MacBook",
					Slug = "MacBook", Description = "Thuong hieu noi tieng the gioi", Status = 1 };
				CategoryModel ROG = new CategoryModel { Name = "Asus",
					Slug = "ROG", Description = "Thuong hieu noi tieng the gioi", Status = 1 };
				CategoryModel Dell = new CategoryModel { Name = "Dell",
					Slug = "Dell", Description = "Thuong hieu noi tieng the gioi", Status = 1 };
				CategoryModel MSI = new CategoryModel { Name = "MSI",
					Slug = "MSI", Description = "Thuong hieu noi tieng the gioi", Status = 1 };
				CategoryModel Lenovo = new CategoryModel { Name = "Lenovo",
					Slug = "Lenovo", Description = "Thuong hieu noi tieng the gioi", Status = 1 };
				
				
				BrandModel Apple = new BrandModel { Name = "MacBook",
					slug = "MacBook", Description = "Thuong hieu noi tieng the gioi", status = 1 };
				BrandModel Asus = new BrandModel { Name = "Asus",
					slug = "Asus", Description = "Thuong hieu noi tieng the gioi", status = 1 };
				BrandModel DELL = new BrandModel { Name = "Dell",
					slug = "Dell", Description = "Thuong hieu noi tieng the gioi", status = 1 };
				BrandModel LENOVO = new BrandModel { Name = "MSI",
					slug = "MSI", Description = "Thuong hieu noi tieng the gioi", status = 1 };

				_context.Products.AddRange(

					new ProductModel { Name = "MacBook", Slug = "MacBook", Description = "Thuong hieu noi tieng the gioi", Imange = "1.jpg", Category = Macbook, Price = 30000000, Brand = Apple },
					new ProductModel { Name = "LENOVO", Slug = "LENOVO", Description = "Thuong hieu noi tieng the gioi", Imange = "1.jpg", Category = Lenovo, Price = 23000000, Brand = LENOVO }
				);
				_context.SaveChanges();
				
			}
		}
	}
}
