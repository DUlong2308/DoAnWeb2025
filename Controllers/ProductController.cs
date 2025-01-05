using MailChimp.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Responsibilty;

namespace WebApplicationlaptop.Controllers
{
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext dataContext)
		{
			_dataContext = dataContext;
		} 
		public IActionResult Index()
		{
			return View();
		}
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Recommend(int productId)
		{
			var product = await _dataContext.Products.Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == productId);
			if (product == null)
			{
				return NotFound();
			}

			var recommendedProducts = await _dataContext.Products
				.Where(p => p.BrandId == product.BrandId && p.Id != productId)
				.ToListAsync();

			return PartialView("_RecommendedItemsPartial", recommendedProducts);
		}
		public IActionResult FeaturedProducts()
		{
			var featuredProducts = _dataContext.Products.Where(p => p.IsFeatured).ToList();
			return View(featuredProducts);
		}
        [HttpPost]
        public async Task<IActionResult> SubmitReview([Bind("Id,Name,Email,Content,ProductId")] ReviewModel review)
        {
            if (review.ProductId == 0)
            {
                return RedirectToAction("Index");
            }

            var product = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)  // Ensure reviews are included
                .FirstOrDefaultAsync(p => p.Id == review.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                _dataContext.Add(review);
                await _dataContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your review has been submitted successfully!";
                return RedirectToAction("Details", new { id = review.ProductId });
            }

            // If ModelState is not valid, re-populate the reviews and return to the Details view with the product model
            product.Reviews = await _dataContext.Reviews.Where(r => r.ProductId == review.ProductId).ToListAsync();
            return View("Details", product);
        }


    }
}
