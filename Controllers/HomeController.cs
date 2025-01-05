using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using WebApplicationlaptop.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WebApplicationlaptop.Responsibility;

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
			// Lấy danh sách sản phẩm từ CSDL, bao gồm cả Category và Brand
			IQueryable<ProductModel> products = _dataContext.Products
				.Include(p => p.Category)
				.Include(p => p.Brand);

			// Nếu có chuỗi tìm kiếm thì lọc danh sách sản phẩm
			if (!string.IsNullOrEmpty(searchString))
			{
				products = products.Where(p => p.Name.Contains(searchString));
			}

			// Nếu có khoảng giá thì lọc danh sách sản phẩm theo khoảng giá
			

			int pageSize = 9; // Số lượng sản phẩm mỗi trang
			int pageNumber = page ?? 1;

			// Sử dụng ToPagedList để phân trang
			var pagedProducts = products.ToPagedList(pageNumber, pageSize);

			// Trả về view với danh sách sản phẩm đã phân trang
			return View(pagedProducts);
		}

		public IActionResult contact()
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
            else
            {
                return View(errorViewModel);
            }
        }
        public IActionResult ThongBaoGuiMail()
        {
            return View();
        }
    }
}
