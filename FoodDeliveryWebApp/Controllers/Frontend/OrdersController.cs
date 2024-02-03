using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(ApplicationDbContext context, IProductRepository productRepository, UserManager<AppUser> userManager)
        {
            _context = context;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> PaymentPage(int productId, int quantity, string userId, string productName, double productPrice, string productImageUrl)
        {
            try
            {
                var product = await _productRepository.GetById(productId);

                // Check if the product is found
                if (product == null)
                {
                    return NotFound(); // Or handle appropriately
                }

                // Create a PaymentViewModel with the retrieved product details
                var orderViewItemModel = new OrderItemViewModel
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    UserId = userId,
                    ProductName = productName,
                    ProductPrice = productPrice,
                    ProductImageUrl = productImageUrl
                };

                // Pass the paymentViewModel to the view
                return View(orderViewItemModel);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal Server Error"); // Or redirect to an error page
            }
        }

        public IActionResult PaymentPageCart()
        {
            return View();
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
            var product = _context.Products.Find(orderViewModel.ProductId);
            var order = new Order
            {
                OrderNo = orderViewModel.OrderNo,
                ProductId = orderViewModel.ProductId,
                Quantity = orderViewModel.Quantity,
                UserId = orderViewModel.UserId,
                Status = orderViewModel.Status,
                PaymentId = null, // We'll set this later after saving the payment
                OrderAt = DateTime.Now,
                Product = product // Include associated product information
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

        

        public async Task<IActionResult> ConfirmationPage(int orderId)
        {
            try
            {
                var orderData = _context.Orders
                    .Include(o => o.Product)
                    .FirstOrDefault(o => o.OrderId == orderId);

                if (orderData == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                // Retrieve user information directly from the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderData.UserId);

                // Create OrderViewModel
                var orderViewModel = new OrderViewModel
                {
                    OrderId = orderData.OrderId,
                    OrderNo = orderData.OrderNo,
                    ProductId = orderData.ProductId,
                    Quantity = orderData.Quantity,
                    PaymentId = orderData.PaymentId,
                    UserId = orderData.UserId,
                    Status = orderData.Status,
                    OrderAt = orderData.OrderAt,
                    UserName = user?.UserName,
                    Phone = user?.Phone,
                    Email = user?.Email,
                    Name = orderData.Product.Name,
                    Price = orderData.Product.Price,
                    OrderItems = new List<OrderItemViewModel>
            {
                // Populate OrderItems as needed
                new OrderItemViewModel
                {
                    ProductId = orderData.Product.ProductId,
                    ProductName = orderData.Product.Name,
                    ProductImageUrl = orderData.Product.ImageUrl,
                    ProductPrice = orderData.Product.Price,
                    UserId = orderData.UserId,
                    Quantity = orderData.Quantity,
                }
                // Add more items as necessary
            }
                };

                // Pass the orderViewModel to the view
                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                // Handle exceptions
                return Json(new { success = false, message = "Error completing order" });
            }
        }





    }
}
