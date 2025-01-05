//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using WebApplicationlaptop.Models;
//using WebApplicationlaptop.Models.ViewModel;

//namespace WebApplicationlaptop.Controllers
//{
//	public class AccountController : Controller
//	{
//		private readonly UserManager<LoginModel> _userManager;
//		private readonly SignInManager<LoginModel> _signInManager;

//		public AccountController(UserManager<LoginModel> userManager, SignInManager<LoginModel> signInManager)
//		{
//			_userManager = userManager;
//			_signInManager = signInManager;
//		}

//		[HttpGet]
//		public IActionResult Register()
//		{
//			return View();
//		}

//		[HttpPost]
//		[ValidateAntiForgeryToken]
//		public async Task<IActionResult> Register(UserModel model)
//		{
//			if (ModelState.IsValid)
//			{
//				var user = new LoginModel { UserName = model.UserName, Email = model.Email };
//				var result = await _userManager.CreateAsync(user, model.Password);
//				if (result.Succeeded)
//				{
//					await _signInManager.SignInAsync(user, isPersistent: false);
//					return RedirectToAction("Login", "Home");
//				}
//				foreach (var error in result.Errors)
//				{
//					ModelState.AddModelError("", error.Description);
//				}
//			}
//			return View(model);
//		}

//		[HttpGet]
//		public IActionResult Login(string returnUrl = null)
//		{
//			return View(new LoginViewModel { ReturnUrl = returnUrl });
//		}

//		[HttpPost]
//		[ValidateAntiForgeryToken]
//		public async Task<IActionResult> Login(LoginViewModel model)
//		{
//			if (ModelState.IsValid)
//			{
//				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
//				if (result.Succeeded)
//				{
//					return RedirectToLocal(model.ReturnUrl);
//				}
//				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//				return View(model);
//			}
//			return View(model);
//		}

//		[HttpPost]
//		[ValidateAntiForgeryToken]
//		public async Task<IActionResult> Logout()
//		{
//			await _signInManager.SignOutAsync();
//			return RedirectToAction("Index", "Home");
//		}

//		private IActionResult RedirectToLocal(string returnUrl)
//		{
//			if (Url.IsLocalUrl(returnUrl))
//			{
//				return Redirect(returnUrl);
//			}
//			return RedirectToAction(nameof(HomeController.Index), "Home");
//		}
//	}
//}
