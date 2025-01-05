using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;

namespace YourNamespace.ViewComponents
{
    public class FeaturedProductsViewComponent : ViewComponent
    {
        private readonly DataContext Context;

        public FeaturedProductsViewComponent(DataContext context)
        {
            Context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await Context.Categories
                .Include(c => c.Products.Where(p => p.IsFeatured)) // Lấy ra sản phẩm có IsFeatured == true
                .ToListAsync();

            return View(categories);
        }
    }
}