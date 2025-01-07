using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using WebApplicationlaptop.Helper;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Models.Momo;
using WebApplicationlaptop.Services.Momo;
using Newtonsoft.Json;
using Azure.Core;
using WebApplicationlaptop.Responsibility;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Migrations;
using System.Globalization;

namespace WebApplicationlaptop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMomoService _momoService;

        public CheckoutController(DataContext dataContext, UserManager<ApplicationUser> userManager, IMomoService momoService)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _momoService = momoService; // Khởi tạo _momoService
        }

        public async Task<IActionResult> Checkout(string OrderId)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin người dùng
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Tạo mã đơn hàng mới
            var orderCode = Guid.NewGuid().ToString();

            // Lấy thông tin từ cookie
            var couponCode = Request.Cookies["CouponCode"];
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            var address = Request.Cookies["fullAddress"];
            var discountAmountCookie = Request.Cookies["DiscountAmount"];
            decimal shippingPrice = 0;
            decimal discountAmount = 0; // Khai báo biến discountAmount

            // Kiểm tra và chuyển đổi giá trị cookie thành decimal
            if (!string.IsNullOrEmpty(shippingPriceCookie))
            {
                decimal.TryParse(shippingPriceCookie, NumberStyles.Any, CultureInfo.InvariantCulture, out shippingPrice);
            }
            // Kiểm tra và chuyển đổi discount amount từ cookie
            if (!string.IsNullOrEmpty(discountAmountCookie))
            {
                decimal.TryParse(discountAmountCookie, out discountAmount);
            }

            // Tạo bản ghi đơn hàng chính
            var orderItem = new OrderModel
            {
                ShippingCost = shippingPrice,
                OrderCode = orderCode,
                UserName = userEmail,
                Status = 0,
                CreateDate = DateTime.Now,
                CouponCode = couponCode,
                PaymentMethod = OrderId ?? "COD",
                DiscountAmount = discountAmount // Gán discountAmount vào OrderModel
            };

            // Lưu thông tin đơn hàng vào cơ sở dữ liệu
            _dataContext.Add(orderItem);
            await _dataContext.SaveChangesAsync();

            // Lấy thông tin giỏ hàng từ session


            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Duyệt qua từng mặt hàng trong giỏ hàng để tạo chi tiết đơn hàng
            foreach (var cart in cartItems)
            {
                var product = _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstOrDefault();
                product.Quantity -= cart.Quantity;
                
                product.Sold += cart.Quantity;

                var orderDetails = new OrderDetail
                {
                    UserName = userEmail,
                    OrderCode = orderCode,
                    ShippingCost = shippingPrice,
                    ProductId = cart.ProductId,
                    ProductName = cart.ProductName,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                    OrderModelId = orderItem.Id,
                    PhoneNumber = user.PhoneNumber,
                    Fullname = user.FullName,
                    Address = address, // Lưu địa chỉ vào thuộc tính Address
                    DiscountAmount = discountAmount
                };

                // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
                _dataContext.Add(orderDetails);
            }

            // Lưu tất cả thay đổi vào cơ sở dữ liệu
            await _dataContext.SaveChangesAsync();


            // Xóa giỏ hàng sau khi thanh toán thành công
            HttpContext.Session.Remove("Cart");

            // Gửi email xác nhận đơn hàng
            await SendMail.SendOrderSummaryEmailAsync(orderItem);
            TempData["success"] = "Đã Thanh toán thành công ";

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("Index", "Cart");
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            // Gọi dịch vụ MoMo để thực hiện thanh toán
            var response = await _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            // Kiểm tra mã kết quả từ MoMo
            if (requestQuery["resultCode"] == "0") // resultCode là chuỗi
            {
                // Tạo đối tượng MomoInfoModel mới
                var newMomoInsert = new MomoInfoModel
                {
                    OrderId = requestQuery["orderId"].ToString(), // Chuyển đổi sang string
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["amount"]), // Chú ý: "Amount" có thể là "amount"
                    OrderInfo = requestQuery["orderInfo"].ToString(),
                    DatePaid = DateTime.Now
                };

                // Thêm vào cơ sở dữ liệu
                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();

                // Lấy địa chỉ từ session hoặc từ cơ sở dữ liệu
                var address = HttpContext.Session.GetString("FullAddress"); // Giả sử bạn đã lưu địa chỉ vào session

                // Kiểm tra xem địa chỉ có tồn tại không
                if (string.IsNullOrEmpty(address))
                {
                    // Xử lý trường hợp không có địa chỉ
                    TempData["error"] = "Không tìm thấy địa chỉ giao hàng.";
                    return RedirectToAction("Index", "Cart");
                }

                // Lưu vào order
                await Checkout(requestQuery["orderId"]); // Gọi phương thức Checkout với cả hai tham số
            }
            else
            {
                // Nếu giao dịch bị hủy
                TempData["success"] = "Đã hủy giao dịch MoMo.";
                return RedirectToAction("Index", "Cart");
            }

            // Trả về kết quả checkout
            return View(response);
        }
    }
}