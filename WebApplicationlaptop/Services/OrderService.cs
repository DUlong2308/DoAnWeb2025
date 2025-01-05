using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context; // Thay YourDbContext bằng tên DbContext của bạn

        public OrderService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<OrderModel>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserName == userId) // Giả sử UserName là tên người dùng
                .Include(o => o.OrderDetail) // Bao gồm chi tiết đơn hàng
                .ToListAsync();
        }
    }
}