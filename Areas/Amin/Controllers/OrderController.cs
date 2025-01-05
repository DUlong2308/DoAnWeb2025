using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationlaptop.Models;
using WebApplicationlaptop.Responsibility;

namespace WebApplicationlaptop.Areas.Amin.Controllers
{
    [Area("Amin")]
    [Authorize(Roles = "Admin,Manager")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index()
        {



            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }

        public async Task<ActionResult<OrderModel>> ViewOrder(int id)
        {
            var order = await _dataContext.Orders.Where(p => p.Id == id).FirstOrDefaultAsync();
            List<OrderDetail> lstOrderDetail = new List<OrderDetail>();
            long _id = Convert.ToInt64(id);
            lstOrderDetail = _dataContext.OrderDetails.Where(s => s.OrderModelId == _id).ToList();

            List<CartItemModel> listCart = new List<CartItemModel>();

            foreach (var item in lstOrderDetail)
            {
                CartItemModel it = new CartItemModel();
                it.ProductId = item.ProductId;
                it.Quantity = item.Quantity;
                ProductModel p = new ProductModel ();
                p = _dataContext.Products.Find(it.ProductId);
                it.Price = p.Price;
                it.ProductName = p.Name;
                it.Imange = p.Imange;
                listCart.Add(it);
            }
            ViewBag.order = order;


            if (order == null)
            {
                return NotFound();
            }

            return View(listCart);
        }

        public async Task<IActionResult> Conform(int id)
        {
            var order = await _dataContext.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status to 0 (assuming 0 means confirmed or processed)
            order.Status = 0;

            // Save changes to the database
            _dataContext.Update(order);
            await _dataContext.SaveChangesAsync();

            // Redirect back to the index action (or return a partial view if using AJAX)
            return RedirectToAction(nameof(Index));
        }
    }
}
