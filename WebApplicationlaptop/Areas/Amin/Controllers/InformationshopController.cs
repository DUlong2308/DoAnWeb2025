using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin")]
    public class InformationshopController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;

        public InformationshopController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var informationshops = await _dataContext.Informationshops.ToListAsync();
            return View(informationshops);
        }

        // GET: Informationshop/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Informationshop/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InformationshopModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle Image Upload
                if (model.ImageUpload != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", model.ImageUpload.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(stream);
                    }
                    model.Logo = "/images/" + model.ImageUpload.FileName; // Save the path to the Logo property
                }

                _dataContext.Informationshops.Add(model);
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Thêm thông tin shop thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Informationshop/Edit/5
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit()
        {
            // Lấy thông tin shop đầu tiên từ cơ sở dữ liệu
            var informationshop = await _dataContext.Informationshops.FirstOrDefaultAsync();

            if (informationshop == null)
            {
                TempData["error"] = "Không tìm thấy thông tin shop.";
                return RedirectToAction(nameof(Index));
            }

            return View(informationshop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(InformationshopModel informationshop)
        {
            // Lấy thông tin shop hiện tại từ cơ sở dữ liệu
            var existingInformationshop = await _dataContext.Informationshops.FirstOrDefaultAsync();

            if (existingInformationshop == null)
            {
                TempData["error"] = "Không tìm thấy thông tin shop.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload hình ảnh nếu có
                    if (informationshop.ImageUpload != null)
                    {
                        // Đường dẫn thư mục lưu ảnh
                        string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "media", "logo");

                        // Kiểm tra thư mục có tồn tại không, nếu chưa tạo
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }

                        // Tạo tên file mới với GUID để tránh trùng lặp
                        string imageName = Guid.NewGuid().ToString() + "-" + informationshop.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadFolder, imageName);

                        // Lưu file vào thư mục
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await informationshop.ImageUpload.CopyToAsync(fs);
                        }

                        // Cập nhật đường dẫn logo mới
                        existingInformationshop.Logo = "media/logo/" + imageName;
                    }

                    // Cập nhật các trường khác
                    existingInformationshop.Name = informationshop.Name;
                    existingInformationshop.Email = informationshop.Email;
                    existingInformationshop.Phone = informationshop.Phone;
                    existingInformationshop.Map = informationshop.Map;
                    existingInformationshop.Description = informationshop.Description;

                    // Cập nhật dữ liệu trong database
                    _dataContext.Update(existingInformationshop);
                    await _dataContext.SaveChangesAsync();

                    TempData["success"] = "Cập nhật thông tin shop thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu xảy ra
                    TempData["error"] = "Lỗi khi cập nhật thông tin: " + ex.Message;
                }
            }
            else
            {
                // Thu thập lỗi từ ModelState
                List<string> errors = new List<string>();
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                TempData["errorMessage"] = string.Join("\n", errors);
            }

            return View(existingInformationshop);
        }



    }
}
