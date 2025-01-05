using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

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

        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> SubmitReview([Bind("Id,Name,Email,Content,ProductId,Rating")] ReviewModel review)
        {
            // Kiểm tra nếu ProductId là 0 (không hợp lệ)
            if (review.ProductId == 0)
            {
                return RedirectToAction("Index");
            }

            // Tìm sản phẩm tương ứng trong cơ sở dữ liệu
            var product = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)  // Đảm bảo rằng reviews đã được load vào
                .FirstOrDefaultAsync(p => p.Id == review.ProductId);

            // Nếu sản phẩm không tồn tại, trả về lỗi 404
            if (product == null)
            {
                return NotFound();
            }

            // Nếu ModelState hợp lệ (đảm bảo dữ liệu đúng)
            if (ModelState.IsValid)
            {
                // Gán thời gian tạo review
                review.CreatedAt = DateTime.Now;

                // Thêm review vào cơ sở dữ liệu
                _dataContext.Reviews.Add(review);
                await _dataContext.SaveChangesAsync();

                // Thông báo thành công và chuyển đến trang chi tiết sản phẩm
                TempData["SuccessMessage"] = "Your review has been submitted successfully!";
                return RedirectToAction("Details", new { id = review.ProductId });
            }

            // Nếu ModelState không hợp lệ, load lại thông tin sản phẩm cùng với các review cũ
            product.Reviews = await _dataContext.Reviews.Where(r => r.ProductId == review.ProductId).ToListAsync();
            return View("Details", product);
        }


        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Kiểm tra nếu sản phẩm có tồn tại trong cơ sở dữ liệu
            var product = await _dataContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm vào giỏ hàng
                cart.Add(new CartItemModel(product));
            }
            else
            {
                // Nếu đã có, tăng số lượng lên 1
                cartItem.Quantity += 1;
            }

            // Lưu lại giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cart);

            // Quay lại trang chi tiết sản phẩm
            return RedirectToAction("Details", new { id = productId });
        }
    }
}