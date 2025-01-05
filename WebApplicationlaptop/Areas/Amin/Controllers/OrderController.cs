using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var orders = await _dataContext.Orders
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Lấy tên đầy đủ cho mỗi đơn hàng
            foreach (var order in orders)
            {
                var user = await _userManager.FindByIdAsync(order.UserName); // Giả sử UserName là ID của người dùng
                order.FullName = user?.FullName; // Gán tên đầy đủ vào thuộc tính FullName của đơn hàng
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

            // Cập nhật trạng thái đơn hàng
            order.Status = 0; // Giả sử 0 có nghĩa là đã xác nhận hoặc đã xử lý

            // Lưu thay đổi vào cơ sở dữ liệu
            _dataContext.Orders.Update(order);
            await _dataContext.SaveChangesAsync();

            // Chuyển hướng về danh sách đơn hàng
            return RedirectToAction(nameof(Index));
        }
    }
}