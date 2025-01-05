using Microsoft.AspNetCore.Mvc;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly DataContext _context;

        public ReviewController(DataContext context)
        {
            _context = context;
        }
    }
}