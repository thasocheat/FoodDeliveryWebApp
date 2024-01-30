using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Web;

namespace FoodDeliveryWebApp.Controllers.Frontend
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;
        public OrdersController(ApplicationDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public IActionResult PaymentPage(int productId, int quantity, string userId)
        {
            var product = _productRepository.GetById(productId);

            // Check if the product is found
            if (product == null)
            {
                return NotFound(); // Or handle appropriately
            }

            // Create a PaymentViewModel with the retrieved product details
            var orderViewItemModel = new OrderItemViewModel
            {
                //ProductId = product.ProductId,
                Quantity = quantity,
                UserId = userId,
                //ProductName = product.Name,
                //ProductPrice = product.Price,
                //ProductImageUrl = product.ImageUrl
            };

            // Pass the paymentViewModel to the view
            return View(orderViewItemModel);
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
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Create an Order entity
                Order newOrder = new Order
                {
                    ProductId = orderRequest.ProductId,
                    Quantity = orderRequest.Quantity,
                    UserId = userId,
                    OrderAt = DateTime.Now,
                    Status = "Pending" // Set the initial status based on your workflow
                };

                // Save the order to the database
                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                return Json(new { success = true, orderId = newOrder.OrderId });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                // Handle exceptions
                return Json(new { success = false, message = "Error placing order" });
            }
        }

        [HttpPost]
        public IActionResult ProcessOrder([FromBody] OrderAndPaymentViewModel orderAndPayment)
        {
            try
            {
                // Save order and payment details
                var orderId = SaveOrderAndPayment(orderAndPayment);

                // Return the order ID (or any other relevant data) to the frontend
                return Json(new { orderId = orderId });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return BadRequest($"Error processing order: {ex.Message}");
            }
        }

        // Function to save order and payment details
        private int SaveOrderAndPayment(OrderAndPaymentViewModel orderAndPayment)
        {
            // Save order details
            var orderId = SaveOrder(orderAndPayment.Order);

            // Save payment details
            SavePayment(orderId, orderAndPayment.Payment);

            return orderId;
        }

        // Function to save order details
        private int SaveOrder(OrderViewModel orderViewModel)
        {
            var order = new Order
            {
                OrderNo = orderViewModel.OrderNo,
                ProductId = orderViewModel.ProductId,
                UserId = orderViewModel.UserId,
                Status = orderViewModel.Status,
                PaymentId = null, // We'll set this later after saving the payment
                OrderAt = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return order.OrderId;
        }

        // Function to save payment details
        private void SavePayment(int orderId, PaymentViewModel paymentViewModel)
        {
            var payment = new Payment
            {
                Name = paymentViewModel.Name,
                CardNo = paymentViewModel.CardNo,
                ExpiryDate = paymentViewModel.ExpiryDate,
                CvvNo = paymentViewModel.CvvNo,
                Address = paymentViewModel.Address,
                PaymentMode = paymentViewModel.PaymentMode,
                PayAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Update the corresponding order with the paymentId
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.PaymentId = payment.PaymentId;
                _context.SaveChanges();
            }
        }

        public IActionResult ConfirmationPage(OrderAndPaymentViewModel orderAndPayment)
        {
            var orderData = TempData["OrderData"] as Order;
            try
            {
                // Save order and payment details
                var orderId = SaveOrderAndPayment(orderAndPayment);

                // Return the order ID (or any other relevant data) to the frontend
                return Json(new { orderId = orderId });

                if (orderId == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                // Save the changes to the database
                _context.SaveChanges();

                return Json(new { success = true, message = "Order completed successfully" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                // Handle exceptions
                return Json(new { success = false, message = "Error completing order" });
            }

            return View(orderData);
        }






    }
}
