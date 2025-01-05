using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
	[Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class ContactController : Controller
	{
		private readonly DataContext _dataContext;

		public ContactController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = dataContext;
			
		}
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Contacts.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
