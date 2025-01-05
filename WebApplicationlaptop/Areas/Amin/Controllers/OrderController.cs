using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailChimp.Net.Models;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(DataContext dataContext, UserManager<ApplicationUser> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        // Hiển thị danh sách đơn hàng
        [HttpGet]
        public async Task<IActionResult> Index(string status)
        {
            var ordersQuery = _dataContext.Orders.AsQueryable();

            // Nếu có status thì lọc theo status
            if (!string.IsNullOrEmpty(status))
            {
                if (int.TryParse(status, out var statusValue))
                {
                    ordersQuery = ordersQuery.Where(o => o.Status == statusValue);
                }

               
            }
         
            // Lấy danh sách đơn hàng đã được lọc (hoặc tất cả)
            var orders = await ordersQuery.OrderByDescending(p => p.Id).ToListAsync();

            // Lấy tên đầy đủ cho mỗi đơn hàng
            foreach (var order in orders)
            {
                var user = await _userManager.FindByIdAsync(order.UserName); // Giả sử UserName là ID của người dùng
                order.FullName = user?.FullName;
            }

            return View(orders); // Trả về danh sách đơn hàng
        }





        // Xem chi tiết đơn hàng
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


        // Xác nhận đơn hàng
        public async Task<IActionResult> Conform(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra trạng thái hiện tại của đơn hàng
            if (order.Status == 0) // Chưa xác nhận
            {
                // Cập nhật trạng thái đơn hàng thành "Đã xác nhận" (giả sử giá trị là 1)
                order.Status = 1;

                // Lưu thay đổi vào cơ sở dữ liệu
                _dataContext.Orders.Update(order);
                await _dataContext.SaveChangesAsync();
            }

            // Chuyển hướng về danh sách đơn hàng
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Đã xử lý" (status = 2)
            order.Status = 2;

            // Lưu thay đổi vào cơ sở dữ liệu
            _dataContext.Orders.Update(order);
            await _dataContext.SaveChangesAsync();

            // Chuyển hướng về danh sách đơn hàng
            return RedirectToAction(nameof(Index)); // Hoặc chuyển hướng đến trang chi tiết đơn hàng
        }
    }
}