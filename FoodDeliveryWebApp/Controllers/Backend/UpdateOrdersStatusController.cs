using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    public class UpdateOrdersStatusController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming ApplicationDbContext is your DbContext

        public UpdateOrdersStatusController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult UpdateOrdersStatus()
        {
            try
            {
                var orders = _context.Orders
                    .GroupBy(o => new { o.OrderNo, o.PaymentId })
                    .Select(group => new OrderViewModel
                    {
                        OrderId = group.FirstOrDefault().OrderId,
                        OrderAt = group.First().OrderAt,
                        Price = group.First().Product.Price,
                        Status = group.First().Status,
                        TotalQuantity = group.Sum(o => o.Quantity),
                        TotalAmount = group.Sum(o => o.Quantity * o.Product.Price),
                        Quantity = group.Sum(o => o.Quantity),
                        OrderNo = group.Key.OrderNo,
                        OrderItems = _context.Orders
                            .Where(oi => oi.OrderId == group.FirstOrDefault().OrderId)
                            .Select(oi => new OrderItemViewModel
                            {
                                ProductId = oi.ProductId,
                                ProductName = oi.Product.Name,
                                ProductImageUrl = oi.Product.ImageUrl,
                                ProductPrice = oi.Product.Price,
                                Quantity = oi.Quantity,
                                TotalQuantity = group.Sum(o => o.Quantity),
                                TotalAmount = group.Sum(o => o.Quantity * o.Product.Price),
                            }).ToList()
                    }).ToList();

                

                return View(orders);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"Error retrieving or processing orders: {ex.Message}");
            }

            //try
            //{
            //    var orders = _context.Orders
            //        .GroupBy(o => new { o.OrderNo, o.PaymentId })
            //        .Select(group => new OrderViewModel
            //        {
            //            OrderNo = group.Key.OrderNo,
            //            TotalAmount = group.Sum(o => o.Quantity * o.Product.Price),
            //            Status = group.First().Status,
            //            // Populate other properties as needed
            //        }).ToList();

            //    return View(orders);
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception or handle it appropriately
            //    return StatusCode(500, $"Error retrieving or processing orders: {ex.Message}");
            //}
        }



        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string newStatus)
        {
            try
            {
                // Retrieve the order from the database based on the orderId
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }

                // Update the status of the order
                order.Status = newStatus;

                // Save changes to the database
                _context.SaveChanges();

                return Ok($"Order status updated successfully. New status: {newStatus}");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"Error updating order status: {ex.Message}");
            }
        }




    }
}
