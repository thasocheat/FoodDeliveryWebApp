using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Frontend
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult PaymentPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(Order orderRequest)
        {
            try
            {
                // Validate the order request model
                if (orderRequest == null || !ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid order data" });
                }

                // Retrieve user ID from the authenticated user
                string userId = User.Identity.Name;

                // Create an Order entity
                Order newOrder = new Order
                {
                    ProductId = orderRequest.ProductId,
                    Quantity = orderRequest.Quantity,
                    UserId = userId,
                    // You may need to handle payment details here
                    // Payment = new Payment { ... }
                    OrderAt = DateTime.Now,
                    Status = "Pending" // You may set the initial status based on your workflow
                };

                // Save the order to the database
                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                return Json(new { success = true, message = "Order placed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.Error.WriteLine(ex);

                // Handle exceptions
                return Json(new { success = false, message = "Error placing order" });
            }
        }
    }
}
