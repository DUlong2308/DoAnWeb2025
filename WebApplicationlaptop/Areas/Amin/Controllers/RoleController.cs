using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationlaptop.Areas.Admin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = users.Select(user => new
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> AddRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName;
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoles(string userId, string[] selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToList());

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add to roles");
                return View();
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToList());

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove from roles");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}