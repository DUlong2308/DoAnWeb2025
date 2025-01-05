using Microsoft.AspNetCore.Mvc;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace YourNamespace.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;

        public ContactController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,Name,Email,Subject,Message")] ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                _dataContext.Add(contact);
                await _dataContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }
    }
}