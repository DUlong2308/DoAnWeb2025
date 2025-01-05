using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Responsibilty;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.OrderByDescending(p => p.Id).ToListAsync());
        }

        [HttpGet]
        public IActionResult AddCategories()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategories(CategoryModel Category)
        {
            

            if (ModelState.IsValid)
            {
                Category.Slug = Category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == Category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError(string.Empty, "Danh mục đã có trong dữ liệu.");
                    return View(Category);
                }

             

                _dataContext.Add(Category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm Danh mục thành công";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Lỗi Nhập Liệu";
                List<string> errors = new List<string>();

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                TempData["errorMessage"] = errorMessage;
            }

            return View(Category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa Danh Mục thành công";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");

            return View(category);
        }
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    category.Slug = category.Name.Replace(" ", "-");
                    var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);
                    if (slug != null)
                    {
                        ModelState.AddModelError(string.Empty, "Danh mục đã có trong dữ liệu.");
                        return View(category);
                    }

                  


                    _dataContext.Entry(category).State = EntityState.Modified;
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật Danh mục thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                TempData["error"] = "Lỗi Nhập Liệu";
                List<string> errors = new List<string>();

                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("\n", errors);
                TempData["errorMessage"] = errorMessage;
            }

            return View(category);
        }
        private bool CategoryExists(int id)
        {
            return _dataContext.Categories.Any(p => p.Id == id);
        }
    }
}
