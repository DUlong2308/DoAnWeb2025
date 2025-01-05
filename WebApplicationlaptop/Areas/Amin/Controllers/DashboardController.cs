using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Services;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;
       

        public DashboardController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
            
        }
        public IActionResult Index()
        {
            // Total number of products
            var totalProducts = _dataContext.Products.Count();

            // Total cost price (assuming cost price is stored in Product table)
            var totalCostPrice = _dataContext.Products
                .AsEnumerable()
                .Sum(p => p.Quantity * p.CostPrice);

            // Total number of orders
            var totalOrders = _dataContext.Orders.Count();

            // Total revenue (assuming Price field in OrderDetail represents revenue)
            var totalRevenue = _dataContext.OrderDetails.Sum(o => o.Quantity * o.Price);

            // Total contacts (assuming Contact is another entity)
            var totalContacts = _dataContext.Contacts.Count();

            // Active coupons (assuming status 1 means active)
            var activeCoupons = _dataContext.Coupons.Count(c => c.Status == 1);

            // Average reviews (assuming Rating field in Review)
            var averageReviews = _dataContext.Reviews.Average(r => r.Rating);

            // Calculate products by category
            var productsByCategory = _dataContext.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = _dataContext.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            // Lấy ngày hiện tại
            var today = DateTime.Today;

            // Tạo danh sách để lưu số lượng đơn hàng cho từng ngày
            var ordersCountByDate = new List<int>();
            var dates = new List<string>();

            // Lặp qua 7 ngày từ cũ đến mới
            for (int i = 6; i >= 0; i--) // Bắt đầu từ 6 đến 0
            {
                var date = today.AddDays(-i); // Tính ngày
                dates.Add(date.ToString("dd/MM")); // Thêm ngày vào danh sách

                // Truy vấn số lượng đơn hàng cho ngày này
                var ordersCount = _dataContext.Orders
                    .Count(o => o.CreateDate.Date == date);

                ordersCountByDate.Add(ordersCount); // Thêm số lượng đơn hàng vào danh sách
            }


            // Send data to the view
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCostPrice = totalCostPrice;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalContacts = totalContacts;
            ViewBag.ActiveCoupons = activeCoupons;
            ViewBag.AverageReviews = averageReviews;
            ViewBag.ProductsByCategory = productsByCategory;
            ViewBag.OrdersCountByDate = ordersCountByDate; // Số lượng đơn hàng cho từng ngày
            ViewBag.Dates = dates; // Danh sách ngày

            return View();
        }
    }
}
