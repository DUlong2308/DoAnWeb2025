using Microsoft.AspNetCore.Mvc;

namespace WebApplicationlaptop.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}