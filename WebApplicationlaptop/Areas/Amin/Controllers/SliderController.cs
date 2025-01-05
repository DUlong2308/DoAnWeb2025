using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class SliderController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _dataContext;

        public SliderController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Sliders.ToListAsync());
        }

        // GET: Slider/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Slider/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ImageUpload != null)
                {
                    var fileName = Path.GetFileName(slider.ImageUpload.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await slider.ImageUpload.CopyToAsync(stream);
                    }

                    slider.Image = fileName; // Lưu tên file vào thuộc tính Image
                }

                _dataContext.Add(slider);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Slider/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _dataContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Slider/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SliderModel slider)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (slider.ImageUpload != null)
                    {
                        var fileName = Path.GetFileName(slider.ImageUpload.FileName);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await slider.ImageUpload.CopyToAsync(stream);
                        }

                        slider.Image = fileName; // Cập nhật tên file
                    }

                    _dataContext.Update(slider);
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Slider/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _dataContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Slider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _dataContext.Sliders.FindAsync(id);
            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _dataContext.Sliders.Any(e => e.Id == id);
        }
    }
}