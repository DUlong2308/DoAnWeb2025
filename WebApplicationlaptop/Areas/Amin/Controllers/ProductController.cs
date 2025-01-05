using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using X.PagedList;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;

        public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {
            IQueryable<ProductModel> products = _dataContext.Products
               .Include(p => p.Category)
               .Include(p => p.Brand);

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            int pageSize = 9; // Số lượng sản phẩm mỗi trang
            int pageNumber = page ?? 1;

            // Sử dụng ToPagedList để phân trang
            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            // Trả về view với danh sách sản phẩm đã phân trang

            return View(pagedProducts);
        }

        [HttpGet]
        public IActionResult AddProducts()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError(string.Empty, "Sản phẩm đã có trong dữ liệu.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string upload = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                    string imageName = Guid.NewGuid().ToString() + "-" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(upload, imageName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    product.Imange = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
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

            return View(product);
        }

        [HttpGet("Amin/Product/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        [HttpPost("Amin/Product/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    product.Slug = product.Name.Replace(" ", "-");
                    var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);
                    if (slug != null)
                    {
                        ModelState.AddModelError(string.Empty, "Sản phẩm đã có trong dữ liệu.");
                        return View(product);
                    }

                    if (product.ImageUpload != null)
                    {
                        string upload = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                        string imageName = Guid.NewGuid().ToString() + "-" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(upload, imageName);

                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageUpload.CopyToAsync(fs);
                        }

                        product.Imange = imageName;
                    }
                    else
                    {
                        product.Imange = existingProduct.Imange;
                    }

                    _dataContext.Entry(product).State = EntityState.Modified;
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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

            return View(product);
        }

        [HttpGet("Amin/Product/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.Imange))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", product.Imange);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _dataContext.Products.Any(p => p.Id == id);
        }
        //addquantity


        [HttpGet]
        public async Task<IActionResult> AddQuantity(int id)
        {
            // Lấy danh sách số lượng sản phẩm đã thêm trước đó từ cơ sở dữ liệu
            var productByQuantity = await _dataContext.ProductQuantities
                .Where(p => p.ProductId == id)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            // Gán danh sách này vào ViewBag
            ViewBag.ProductByQuantity = productByQuantity;
            ViewBag.Id = id; // Truyền ID sản phẩm vào ViewBag

            return View();
        }

        [Route("StoreProductQuantity")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel )
        {
            if (productQuantityModel == null || productQuantityModel.ProductId <= 0)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Lấy sản phẩm cần cập nhật
            var product = _dataContext.Products.Find(productQuantityModel.ProductId);
            if (product == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

          
            product.Quantity += productQuantityModel.Quantity;

            // Thêm thông tin số lượng mới vào bảng ProductQuantityModel (nếu có)
            productQuantityModel.Quantity = productQuantityModel.Quantity;
            productQuantityModel.ProductId = productQuantityModel.ProductId;
            productQuantityModel.DateCreated = DateTime.Now;
           
            
            _dataContext.ProductQuantities.Add(productQuantityModel);

            _dataContext.SaveChanges();

            TempData["success"] = "Thêm số lượng sản phẩm thành công.";

            // Chuyển hướng đến trang chi tiết hoặc danh sách sản phẩm
            return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductId });
        }

    }
}