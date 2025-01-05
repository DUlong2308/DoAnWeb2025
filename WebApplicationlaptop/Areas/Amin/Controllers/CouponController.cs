using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly DataContext _dataContext;

        public CouponController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index()
        {
            var coupons = await _dataContext.Coupons.ToListAsync();
            return View(coupons);
        }

        // GET: Coupon/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel coupon)
        {
            // Kiểm tra phần trăm giảm giá
            if (coupon.DiscountPercentage < 0 || coupon.DiscountPercentage > 100)
            {
                ModelState.AddModelError("DiscountPercentage", "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100.");
            }

            // Kiểm tra số tiền giảm giá
            if (coupon.PriceDecrease < 0)
            {
                ModelState.AddModelError("PriceDecrease", "Số tiền giảm giá phải lớn hơn hoặc bằng 0.");
            }

            // Nếu tất cả thông tin hợp lệ, lưu coupon và chuyển hướng về trang danh sách
            if (ModelState.IsValid)
            {
                _dataContext.Add(coupon);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm Code giảm giá thành công";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Nếu có lỗi, xử lý và thông báo lỗi cho người dùng
                TempData["error"] = "Lỗi Nhập Liệu";

                // Lấy tất cả các lỗi từ ModelState và kết hợp thành chuỗi
                var errors = ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage)
                    .ToList();

                string errorMessage = string.Join("\n", errors);
                TempData["errorMessage"] = errorMessage;
            }

            // Trả về lại view với coupon để người dùng có thể sửa thông tin
            return View(coupon);
        }

    }
}
