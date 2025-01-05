using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationlaptop.Models;

namespace WebApplicationlaptop.Services
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetOrdersByUserIdAsync(string userId);
    }
}