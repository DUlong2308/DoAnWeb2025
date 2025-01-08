using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                try
                {
                    // Tạo yêu cầu thanh toán MoMo
                    var response = await _momoService.CreatePaymentMomo(model);

                    // Kiểm tra xem PayUrl có hợp lệ không
                    if (!string.IsNullOrEmpty(response.PayUrl))
                    {
                        // Chuyển hướng đến trang thanh toán MoMo
                        return Redirect(response.PayUrl);
                    }
                    else
                    {
                        // Xử lý trường hợp PayUrl không hợp lệ
                        ModelState.AddModelError("", "Không thể chuyển hướng đến trang thanh toán. Vui lòng thử lại.");
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi và trả về thông báo lỗi
                    ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
                }
            }

            return View(model); // Trả về lại view nếu model không hợp lệ hoặc có lỗi
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = await _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response); // Đảm bảo rằng bạn có view tương ứng để hiển thị kết quả
        }
    }
}