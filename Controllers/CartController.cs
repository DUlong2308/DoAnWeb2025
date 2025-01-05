using Microsoft.AspNetCore.Mvc;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Models.ViewModel;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Responsibilty;

namespace WebApplicationlaptop.Controllers
{
	public class CartController : Controller
	{
		private readonly DataContext _dataContext;

		public CartController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}

		public IActionResult Index()
		{
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

			CartItemViewModel cartVm = new()
			{
				CartItems = cartItems,
				GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
			};

			return View(cartVm);
		}

		public ActionResult Checkout()
		{
			return View("~/Views/Checkout/Index.cshtml");
		}

		public async Task<IActionResult> Add(int Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new CartItemModel(product));
			}
			else
			{
				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);
			return Redirect(Request.Headers["Referer"].ToString());
		}

		public IActionResult Increase(int id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem != null)
			{
				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);
			return RedirectToAction("Index");
		}

		public IActionResult Decrease(int id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem != null)
			{
				if (cartItem.Quantity > 1)
				{
					cartItem.Quantity -= 1;
				}
				else
				{
					cart.Remove(cartItem);
				}
			}

			HttpContext.Session.SetJson("Cart", cart);
			return RedirectToAction("Index");
		}

		public IActionResult Remove(int id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

			if (cartItem != null)
			{
				cart.Remove(cartItem);
			}

			HttpContext.Session.SetJson("Cart", cart);
			return RedirectToAction("Index");
		}
        public async Task<IActionResult> Clear()
		{
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

    }
}