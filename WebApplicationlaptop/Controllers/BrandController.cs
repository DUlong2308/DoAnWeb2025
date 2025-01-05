using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index(string slug = "")
        {
            BrandModel Brand = _dataContext.Brands.Where(c => c.slug == slug).FirstOrDefault();
            if (Brand == null) return RedirectToAction("Index");
            var productsByBrand = _dataContext.Products.Where(p => p.BrandId == Brand.Id);
            return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}