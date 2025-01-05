using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm dòng này
using System.Threading.Tasks;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Models.Momo;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Services.Momo;

namespace WebApplicationlaptop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly DataContext _dataContext;

        public PaymentController(DataContext dataContext, IMomoService momoService)
        {
            _dataContext = dataContext;
            _momoService = momoService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
        {
            if (ModelState.IsValid)
            {
                // Tạo yêu cầu thanh toán MoMo
                var response = await _momoService.CreatePaymentMomo(model);

                // Kiểm tra xem PayUrl có hợp lệ không
                if (!string.IsNullOrEmpty(response.PayUrl))
                {
                    // Giảm số lượng mã giảm giá nếu có
                    if (!string.IsNullOrEmpty(model.CouponCode))
                    {
                        var coupon = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Name == model.CouponCode);
                        if (coupon != null && coupon.Quantity > 0)
                        {
                            coupon.Quantity--; // Giảm số lượng mã giảm giá
                            await _dataContext.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                        }
                    }

                    // Chuyển hướng đến trang thanh toán MoMo
                    return Redirect(response.PayUrl);
                }
                else
                {
                    // Xử lý trường hợp PayUrl không hợp lệ
                    ModelState.AddModelError("", "Không thể chuyển hướng đến trang thanh toán. Vui lòng thử lại.");
                    return View(model); // Trả về lại view với thông báo lỗi
                }
            }

            return View(model); // Trả về lại view nếu model không hợp lệ
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }
    }
}