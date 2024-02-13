using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using System.Threading.Tasks;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(ApplicationDbContext context, IProductRepository productRepository, UserManager<AppUser> userManager, IOrderRepository orderRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
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

        public async Task<IActionResult> PaymentPageCart(string cartItems)
        {
            try
            {
                if (string.IsNullOrEmpty(cartItems))
                {
                    // Handle empty cart appropriately, such as redirecting to the cart page
                    //return RedirectToAction("GetCartProducts");
                    return NotFound(new { success = false, message = "Cart items not found" });
                }

                // Parse the JSON string containing cart items into a list of CartViewModel
                var cartItemsList = JsonConvert.DeserializeObject<List<CartViewModel>>(cartItems);

                if (cartItemsList == null || cartItemsList.Count == 0)
                {
                    // Handle empty cart appropriately, such as redirecting to the cart page
                    //return RedirectToAction("GetCartProducts");
                    return NotFound(new { success = false, message = "Empty cart items" });
                }

                //var orderViewModels = new List<OrderViewModel>();

                //// Iterate through each cart item to create order view models
                //foreach (var cartItem in cartItemsList)
                //{
                //    var product = await _productRepository.GetById(cartItem.ProductId);

                //    // Check if the product is found
                //    if (product == null)
                //    {
                //        // Log or handle the scenario where the product is not found
                //        continue; // Skip to the next cart item
                //    }

                //    // Create an OrderViewModel for the current cart item
                //    var orderViewModel = new OrderViewModel
                //    {
                //        ProductId = product.ProductId,
                //        Quantity = cartItem.Quantity,
                //        UserId = cartItem.UserId,
                //        Name = product.Name,
                //        Price = product.Price, // Assuming Price is the correct property
                //        ImageUrl = product.ImageUrl
                //    };

                //    orderViewModels.Add(orderViewModel);
                //}

                //// Save orderViewModels to the database or perform further processing
                //await _orderRepository.CreateOrders(orderViewModels);

                // Optionally, clear the cart after placing the order
                // ClearCart();

                // Pass the orderViewModels to the payment page view
                return View(cartItemsList);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal Server Error"); // Or redirect to an error page
            }
        }



        //public ActionResult PaymentPageCart()
        //{
        //    return View();
        //}




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



        //public async Task<IActionResult> ConfirmationPage(int orderId)
        //{
        //    try
        //    {
        //        var orderData = _context.Orders
        //            .Include(o => o.Product)
        //            .FirstOrDefault(o => o.OrderId == orderId);

        //        if (orderData == null)
        //        {
        //            return NotFound(new { success = false, message = "Order not found" });
        //        }

        //        // Retrieve user information directly from the database
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderData.UserId);

        //        // Create OrderViewModel
        //        var orderViewModel = new OrderViewModel
        //        {
        //            OrderId = orderData.OrderId,
        //            OrderNo = orderData.OrderNo,
        //            ProductId = orderData.ProductId,
        //            Quantity = orderData.Quantity,
        //            PaymentId = orderData.PaymentId,
        //            UserId = orderData.UserId,
        //            Status = orderData.Status,
        //            OrderAt = orderData.OrderAt,
        //            UserName = user?.UserName,
        //            Phone = user?.Phone,
        //            Email = user?.Email,
        //            Name = orderData.Product.Name,
        //            Price = orderData.Product.Price,
        //            OrderItems = new List<OrderItemViewModel>
        //    {
        //        // Populate OrderItems as needed
        //        new OrderItemViewModel
        //        {
        //            ProductId = orderData.Product.ProductId,
        //            ProductName = orderData.Product.Name,
        //            ProductImageUrl = orderData.Product.ImageUrl,
        //            ProductPrice = orderData.Product.Price,
        //            UserId = orderData.UserId,
        //            Quantity = orderData.Quantity,
        //        }
        //        // Add more items as necessary
        //    }
        //        };

        //        // Pass the orderViewModel to the view
        //        return View(orderViewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine(ex);

        //        // Handle exceptions
        //        return Json(new { success = false, message = "Error completing order" });
        //    }
        //}

        // This used to handle both scenarios
        public async Task<IActionResult> ConfirmationPage(int orderId, bool fromCart)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var users = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (users == null)
                {
                    // Handle the case where the user is not found
                    return NotFound(new { success = false, message = "User not found" });
                }
                // Retrieve order data based on orderId
                var orderData = _context.Orders
                    .Include(o => o.Product)
                    .FirstOrDefault(o => o.OrderId == orderId);

                if (orderData == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                // Retrieve all order items associated with the same order number and payment ID
                var orderItems = _context.Orders
                    .Where(o => o.OrderNo == orderData.OrderNo && o.PaymentId == orderData.PaymentId)
                    .Include(o => o.Product)
                    .ToList();

                if (fromCart)
                {
                    
                    // If the order is from the cart, create OrderViewModel with all order items
                    var orderViewModel = new OrderViewModel
                    {
                        UserName = users?.UserName,
                        Phone = users?.Phone,
                        Email = users?.Email,
                        OrderNo = orderData.OrderNo,
                        Status = orderData.Status,

                        OrderItems = orderItems.Select(o => new OrderItemViewModel
                        {
                            ProductId = o.Product.ProductId,
                            ProductName = o.Product.Name,
                            ProductImageUrl = o.Product.ImageUrl,
                            ProductPrice = o.Product.Price,
                            Quantity = o.Quantity
                        }).ToList()
                    };

                    // Pass the orderViewModel to the view
                    return View(orderViewModel);
                }
                else
                {
                    // If the order is from the view detail page, create OrderViewModel with only the single order item
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == orderData.UserId);

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
                    // Populate OrderItems with the single order item
                    new OrderItemViewModel
                    {
                        ProductId = orderData.Product.ProductId,
                        ProductName = orderData.Product.Name,
                        ProductImageUrl = orderData.Product.ImageUrl,
                        ProductPrice = orderData.Product.Price,
                        Quantity = orderData.Quantity,
                    }
                }
                    };

                    // Pass the orderViewModel to the view
                    return View(orderViewModel);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                // Handle exceptions
                return Json(new { success = false, message = "Error completing order" });
            }
        }





        [HttpPost]
        public async Task<IActionResult> ProcessOrderCart([FromBody] OrderAndPaymentViewModel orderAndPayment)
        {
            try
            {
                // Extract payment data and cart items data from the model
                var cartItems = orderAndPayment.Orders;
                var paymentData = orderAndPayment.Payment;

                // Save payment data and cart items data into respective tables
                var orderId = await SavePaymentAndOrdersToDatabase(paymentData, cartItems);

                // Remove cart items associated with the current user
                await RemoveCartItemsForCurrentUser(HttpContext);

                // Optionally, return a success response
                return Ok(new {orderId});
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"Error processing order: {ex.Message}");
            }
        }

        private async Task<int> SavePaymentAndOrdersToDatabase(PaymentViewModel paymentData, List<OrderViewModel> orderViewModels)
        {
            int orderId = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Save payment data into the payment table
                    var paymentId = await SavePaymentToDatabase(paymentData);

                    // Save payment data into the Payment table
                    //await SavePaymentToDatabase(paymentData);

                    // Save order data into the Orders table
                    orderId = await SaveOrdersToDatabase(orderViewModels, paymentId);

                    transaction.Commit(); // Commit the transaction if everything succeeds
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if an error occurs
                    transaction.Rollback();
                    throw ex; // Rethrow the exception for higher-level handling
                }
            }

            return orderId;
        }

        private async Task<int> SaveOrdersToDatabase(List<OrderViewModel> orderViewModels, int paymentId)
        {
            int orderId = 0;
            // Iterate through each order view model and save it to the database
            foreach (var orderViewModel in orderViewModels)
            {
                var product = await _context.Products.FindAsync(orderViewModel.ProductId);

                if (product != null)
                {
                    var order = new Order
                    {
                        OrderNo = orderViewModel.OrderNo,
                        ProductId = orderViewModel.ProductId,
                        Quantity = orderViewModel.Quantity,
                        UserId = orderViewModel.UserId,
                        Status = orderViewModel.Status,
                        PaymentId = paymentId,
                        OrderAt = DateTime.Now,
                        Product = product // Include associated product information
                    };

                    _context.Orders.Add(order);

                    // Save changes to get the order ID
                    await _context.SaveChangesAsync();

                    // Set orderId to the newly generated order ID
                    orderId = order.OrderId;
                }
                else
                {
                    // Log or handle the scenario where the product associated with the order is not found
                }
            }

            //await _context.SaveChangesAsync();
            return orderId;
        }

        private async Task<int> SavePaymentToDatabase(PaymentViewModel paymentData)
        {
            var payment = new Payment
            {
                Name = paymentData.Name,
                CardNo = paymentData.CardNo,
                ExpiryDate = paymentData.ExpiryDate,
                CvvNo = paymentData.CvvNo,
                Address = paymentData.Address,
                PaymentMode = paymentData.PaymentMode,
                PayAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment.PaymentId;
        }


        private async Task RemoveCartItemsForCurrentUser(HttpContext httpContext)
        {
            // Get the current user's ID
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                // Find cart items for the current user
                var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();

                // Remove cart items from the database
                _context.Carts.RemoveRange(cartItems);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IActionResult> ConfirmationPageCart(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            // Retrieve the order details from the database based on the order ID
            var order = await _context.Orders
                .Include(o => o.OrderNo)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Pass the order details to the confirmation view
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrder(int orderId)
        {
            try
            {
                // Retrieve the user ID of the currently logged-in user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var orderData = await _context.Orders
                    .Include(o => o.AppUser)
                    .Include(o => o.Product)
                    .Where(o => o.OrderId == orderId && o.UserId == userId)
                    .FirstOrDefaultAsync();

                if (orderData == null)
                {
                    return NotFound(new { success = false, message = "Order not found" });
                }

                // Retrieve all products with the same OrderNo
                var relatedProducts = await _context.Orders
                    .Include(o => o.Product)
                    .Where(o => o.OrderNo == orderData.OrderNo && o.UserId == userId)
                    .Select(o => new OrderItemViewModel
                    {
                        ProductId = o.ProductId,
                        ProductName = o.Product.Name,
                        ProductImageUrl = o.Product.ImageUrl,
                        ProductPrice = o.Product.Price,
                        Quantity = o.Quantity
                    })
                    .ToListAsync();

                var orderViewModel = new OrderViewModel
                {
                    OrderId = orderData.OrderId,
                    OrderNo = orderData.OrderNo,
                    UserId = orderData.UserId,
                    Status = orderData.Status,
                    OrderAt = orderData.OrderAt,
                    UserName = orderData.AppUser.UserName,
                    Phone = orderData.AppUser.Phone,
                    Email = orderData.AppUser.Email,
                    OrderItems = relatedProducts
                };

                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return Json(new { success = false, message = "Error viewing order" });
            }
        }














        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            // Retrieve the order from the database
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                // If the order is not found, return a not found response
                return NotFound();
            }

            try
            {
                // Remove the order from the database
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("OrderHistory");
            }
            catch (Exception ex)
            {
                // If there's an error deleting the order, handle the exception
                // Here you can log the exception or return an appropriate error response
                Console.Error.WriteLine(ex);
                return RedirectToAction("OrderHistory"); // Redirect to the order history page
            }
        }










    }
}
