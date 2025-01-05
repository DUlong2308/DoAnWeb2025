using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ShippingController : Controller
    {
        private readonly DataContext _dataContext;
        public ShippingController(DataContext dataContext)
        {
            _dataContext = dataContext;
           
        }
        public async Task<IActionResult> Index()
        {
            
            var shippingList = await _dataContext.Shippings.ToListAsync();

           
            ViewBag.Shippings = shippingList;

            
            return View();
        }
        public async Task<IActionResult> StoreShipping(ShippingModel shippingModel, string phuong, string quan, string tinh, decimal price)
        {
            // Gán giá trị từ tham số vào model
            shippingModel.City = tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;

            try
            {
                // Kiểm tra dữ liệu trùng lặp
                var existingShipping = await _dataContext.Shippings
                    .AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

                if (existingShipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
                }

                // Thêm mới dữ liệu
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();

                return Ok(new { success = true, message = "Thêm shipping thành công." });
            }
            catch (Exception)
            {
                // Xử lý ngoại lệ
                return StatusCode(500, "Đã xảy ra lỗi khi thêm shipping.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteShipping(int id)
        {
            var shipping = await _dataContext.Shippings.FindAsync(id); // Sử dụng FindAsync để tìm kiếm shipping bất đồng bộ
            if (shipping != null)
            {
                _dataContext.Shippings.Remove(shipping);
                await _dataContext.SaveChangesAsync(); // Sử dụng SaveChangesAsync để lưu thay đổi bất đồng bộ
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}
