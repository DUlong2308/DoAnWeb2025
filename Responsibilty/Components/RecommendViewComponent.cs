using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Models; // Ensure you have the correct namespace for your DataContext and Product model

namespace YourNamespace.ViewComponents
{
	public class RecommendViewComponent : ViewComponent
	{
		private readonly DataContext _context;

		public RecommendViewComponent(DataContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync(int productId)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product == null)
			{
				return Content("Product not found.");
			}

			var recommendedProducts = await _context.Products
				.Where(p => p.BrandId == product.BrandId && p.Id != productId)
				.ToListAsync();

			return View(recommendedProducts);
		}
	}
}
