using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WebApplicationlaptop.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(DataContext dataContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Lấy userId và userEmail
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
            var orders = await _dataContext.Orders
                 .Where(od => od.UserName == userEmail) // Sử dụng == thay vì =
                 .OrderByDescending(od => od.Id)
                 .ToListAsync();

            // Gán email của người dùng vào ViewBag để sử dụng trong view
            ViewBag.UserEmail = userEmail;

            // Trả về view với danh sách đơn hàng
            return View(orders);
        }
        public async Task<IActionResult> ViewOrder(int id)
        {
            var order = await _dataContext.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var lstOrderDetail = await _dataContext.OrderDetails
                .Where(od => od.OrderModelId == id)
                .ToListAsync();

            var listCart = new List<CartItemModel>();

            foreach (var item in lstOrderDetail)
            {
                var product = await _dataContext.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var cartItem = new CartItemModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price,
                        ProductName = product.Name,
                        Imange = product.Imange
                    };
                    listCart.Add(cartItem);
                }
            }

            var orderDetail = lstOrderDetail.FirstOrDefault();

            ViewBag.Order = order;
            ViewBag.OrderDetail = orderDetail;

            return View(listCart);
        }
        [AllowAnonymous]
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }).ToList();

            // Lấy thông tin từ claims
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? claims.FirstOrDefault(c => c.Type == "nameidentifier")?.Value;

            var givenName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var surname = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            // Kiểm tra xem người dùng đã tồn tại trong hệ thống chưa
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Nếu người dùng chưa tồn tại, tạo mới
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name,
                    GoogleId = googleId,
                    // Bạn có thể thêm các thuộc tính khác nếu cần
                };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create user.");
                }
             
            }

            // Đăng nhập người dùng
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Chuyển hướng về trang chính hoặc trang yêu cầu
            return RedirectToAction("Index", "Home");
        }
    }
}