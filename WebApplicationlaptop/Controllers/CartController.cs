using Microsoft.AspNetCore.Mvc;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Models.ViewModel;
using WebApplicationlaptop.Responsibility;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplicationlaptop.Migrations;

namespace WebApplicationlaptop.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;

        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            var coupon_code = Request.Cookies["CouponTitle"];
            decimal discountAmount = 0; // Khởi tạo biến giảm giá

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            decimal totalAmount = cartItems.Sum(x => x.Quantity * x.Price);

            // Kiểm tra xem có mã giảm giá không và tính toán giảm giá
            if (!string.IsNullOrEmpty(coupon_code))
            {
                discountAmount = await GetDiscountAmount(coupon_code, totalAmount);
            }

            // Tính tổng tiền bao gồm phí vận chuyển và trừ đi số tiền giảm giá
            decimal grandTotal = totalAmount + shippingPrice - discountAmount;

            CartItemViewModel cartVm = new()
            {
                CartItems = cartItems,
                GrandTotal = grandTotal, // Cập nhật tổng tiền
                ShippingCost = shippingPrice,
                CouponCode = coupon_code,
                DiscountAmount = discountAmount // Thêm thuộc tính DiscountAmount vào mô hình
            };

            return View(cartVm);
        }

        // Phương thức giả định để lấy số tiền giảm giá
        private async Task<decimal> GetDiscountAmount(string couponCode, decimal totalAmount)
        {
            // Tìm coupon trong cơ sở dữ liệu
            var coupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(c => c.Name == couponCode && c.Status == 1 && c.DateStart <= DateTime.Now && c.DateExpired >= DateTime.Now);

            // Nếu không tìm thấy coupon hợp lệ, trả về 0
            if (coupon == null)
            {
                return 0;
            }

            decimal discountAmount = 0;

            // Tính toán số tiền giảm giá dựa trên loại giảm giá
            if (coupon.DiscountType == 1) // Giảm giá theo số tiền
            {
                discountAmount = coupon.PriceDecrease ?? 0;
            }
            else if (coupon.DiscountType == 2) // Giảm giá theo phần trăm
            {
                discountAmount = (decimal)(coupon.DiscountPercentage ?? 0) / 100 * totalAmount;
            }

            // Kiểm tra xem số tiền giảm giá có lớn hơn tổng số tiền không
            if (discountAmount > totalAmount)
            {
                return totalAmount; // Nếu giảm giá lớn hơn tổng số tiền, chỉ giảm tối đa bằng tổng số tiền
            }

            return discountAmount; // Trả về số tiền giảm giá
        }

        public ActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItems == null)
            {
                cart.Add(new CartItemModel(product));
                TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công ";
            }
            else
            {
                cartItems.Quantity += 1;
                TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công ";
            }

            HttpContext.Session.SetJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Increase(int id)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            // Lấy sản phẩm từ cơ sở dữ liệu
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);

            if (cartItem != null && product != null)
            {
                // Kiểm tra nếu số lượng trong giỏ hàng không vượt quá số lượng trong kho
                if (product.Quantity > cartItem.Quantity)
                {
                    // Tăng số lượng trong giỏ hàng
                    cartItem.Quantity += 1;
                    TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công ";
                }
                else
                {
                    // Thêm thông báo nếu không còn đủ sản phẩm trong kho
                    TempData["Warning"] = "Số lượng sản phẩm đạt giới hạn ";
                }
            }

            // Cập nhật lại giỏ hàng trong Session
            HttpContext.Session.SetJson("Cart", cart);

            // Chuyển hướng về trang giỏ hàng
            return RedirectToAction("Index");
        }



        public IActionResult Decrease(int id)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }

            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            return await Task.FromResult(RedirectToAction("Index"));
        }

        // Phương thức GetShipping
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string quan, string phuong, string TINH , string houseNumber)
        {
            // Kiểm tra thông tin địa chỉ
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == TINH && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = 0; // Set mặc định giá trị tiền

            if (existingShipping != null)
            {
                // Nếu tìm thấy giá trị shipping trong database
                shippingPrice = existingShipping.Price;
            }
            else
            {
                // Set giá tiền mặc định nếu không tìm thấy
                shippingPrice = 50000;
            }

            // Chuyển đổi giá trị shippingPrice sang JSON
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);

            // Gửi cookie chứa giá trị shippingPrice
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  // Chỉ có thể truy cập qua HTTP, không thể truy cập qua JavaScript
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),  // Thời gian hết hạn cookie
                    Secure = true     // Chỉ gửi cookie qua HTTPS (bảo mật hơn)
                };

                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return BadRequest("Error setting shipping price cookie: " + ex.Message);
            }

            // Trả về kết quả
            return Json(new { success = true, shippingPrice = shippingPrice });
        }


        [HttpPost]
        public IActionResult DeleteShippingFee()
        {
            try
            {
                // Remove the shipping price from cookies
                Response.Cookies.Delete("ShippingPrice");

                // Optionally, you can also clear it from the session if you are using session storage
                // HttpContext.Session.Remove("ShippingPrice");

                return Ok("Shipping fee deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log the error)
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetCoupon(string coupon_value)
        {
            // Kiểm tra xem coupon có hợp lệ không
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.Quantity >= 1);

            if (validCoupon != null)
            {
                TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;

                // Kiểm tra xem coupon có còn hiệu lực không
                if (remainingTime.Days >= 0)
                {
                    try
                    {
                        decimal totalAmount = GetCurrentTotalAmount(); // Lấy tổng tiền hiện tại
                        decimal discountAmount = 0;

                        // Tính toán giảm giá dựa trên loại coupon
                        if (validCoupon.DiscountType == 1) // Giảm giá theo phần trăm
                        {
                            if (validCoupon.DiscountPercentage.HasValue)
                            {
                                discountAmount = totalAmount * (decimal)(validCoupon.DiscountPercentage.Value / 100);
                            }
                        }
                        else if (validCoupon.DiscountType == 0) // Giảm giá trực tiếp
                        {
                            if (validCoupon.PriceDecrease.HasValue)
                            {
                                discountAmount = validCoupon.PriceDecrease.Value;

                                // Đảm bảo rằng discountAmount không vượt quá tổng tiền
                                if (discountAmount > totalAmount)
                                {
                                    discountAmount = totalAmount; // Giảm tối đa bằng tổng tiền
                                }
                            }
                        }

                        // Tính tổng tiền sau khi áp dụng coupon
                        decimal finalAmount = totalAmount - discountAmount;

                        // Cập nhật số lượng coupon
                        validCoupon.Quantity--;
                        await _dataContext.SaveChangesAsync();

                        // Tạo couponTitle (bao gồm cả tên và mô tả)
                        string couponTitle = validCoupon.Name + " " + validCoupon?.Description;

                        // Lưu couponTitle vào cookie
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30), // Thời gian sống của cookie
                            HttpOnly = true, // Cookie chỉ có thể được truy cập từ server
                            Secure = true, // Chỉ gửi cookie qua HTTPS
                            SameSite = SameSiteMode.Strict // Cài đặt SameSite
                        };
                        Response.Cookies.Append("couponTitle", couponTitle, cookieOptions); // Lưu couponTitle vào cookie

                        // Trả về kết quả thành công
                        return Json(new
                        {
                            success = true,
                            message = "Coupon applied successfully!",
                            discountAmount = discountAmount,
                            finalAmount = finalAmount // Trả về tổng tiền sau khi giảm giá
                        });
                    }
                    catch (Exception ex)
                    {
                        // Ghi log lỗi nếu cần
                        Console.WriteLine($"Error applying coupon: {ex.Message}");
                        return Json(new { success = false, message = "An error occurred while applying the coupon." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Coupon has expired." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Invalid coupon code." });
            }
        }
 
        private decimal GetCurrentTotalAmount()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
          
           

            return cartItems.Sum(x => x.Quantity * x.Price) ;
        }
    }
}
