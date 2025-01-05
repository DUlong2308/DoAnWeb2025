using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;
using X.PagedList;

namespace WebApplicationlaptop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DataContext context, ILogger<HomeController> logger)
        {
            _dataContext = context;
            _logger = logger;
        }

        public IActionResult Index(string searchString, int? page, int? minPrice, int? maxPrice)
        {
            // Thiết lập giá trị mặc định cho minPrice và maxPrice
            minPrice = minPrice ?? 0;
            maxPrice = maxPrice ?? int.MaxValue;

            // Lấy danh sách sản phẩm từ CSDL bao gồm Category và Brand
            IQueryable<ProductModel> products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand);

            // Áp dụng tìm kiếm theo tên sản phẩm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc sản phẩm theo khoảng giá nếu có
            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            // Xử lý phân trang
            int pageSize = 9; // Số sản phẩm mỗi trang
            int pageNumber = page ?? 1; // Trang mặc định là trang 1

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            // Lấy danh sách sliders có trạng thái = 1
            var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
            ViewBag.Sliders = sliders;

            // Truyền giá trị khoảng giá cho view
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Trả về view với dữ liệu phân trang
            return View(pagedProducts);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error(int statusCode)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = statusCode == 404 ? "Page not found" : "An error occurred while processing your request."
            };

            if (statusCode == 404)
            {
                return View("NotFound", errorViewModel);
            }

            return View(errorViewModel);
        }

        public IActionResult ThongBaoGuiMail()
        {
            return View();
        }
    }
}