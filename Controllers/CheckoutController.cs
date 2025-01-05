using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationlaptop.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApplicationlaptop.Responsibility;
using WebApplicationlaptop.Helper;
using System.Linq;

namespace WebApplicationlaptop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;

        public CheckoutController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();

                // Create the main order record
                var orderitem = new OrderModel
                {
                    OrderCode = ordercode,
                    UserName = userEmail,
                    Status = 1,
                    CreateDate = DateTime.Now
                };
                _dataContext.Add(orderitem);
                await _dataContext.SaveChangesAsync();

                // Get the cart items from session
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                // Create order details for each item in the cart
                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetail
                    {
                        UserName = userEmail,
                        OrderCode = ordercode,
                        ProductId = cart.ProductId,
                        ProductName = cart.ProductName,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        OrderModelId = orderitem.Id
                    };
                    _dataContext.Add(orderdetails);
                }
                await _dataContext.SaveChangesAsync();

                // Fetch the complete order with its details to pass to the email template
                var completeOrder = _dataContext.Orders
                    .Where(o => o.Id == orderitem.Id)
                    .Select(o => new OrderModel
                    {
                        Id = o.Id,
                        UserName = o.UserName,
                        OrderCode = o.OrderCode,
                        CreateDate = o.CreateDate,
                        Status = o.Status,
                        OrderDetail = _dataContext.OrderDetails.Where(d => d.OrderModelId == o.Id).ToList()
                    })
                    .FirstOrDefault();

                // Clear the cart from session after successful checkout
                HttpContext.Session.Remove("Cart");

                // Send order confirmation email
                await SendMail.SendEmail2(orderitem);

                // Send orderitem directly

                return RedirectToAction("Index", "Cart");
            }
        }

        private decimal CalculateTotal(List<CartItemModel> cartItems)
        {
            // Calculate the total order amount
            decimal total = 0;
            foreach (var cart in cartItems)
            {
                total += cart.Price * cart.Quantity;
            }
            return total;
        }
    }
}
