using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodDeliveryWebApp.Controllers.Frontend
{
    public class FrontPagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FrontPagesController(ApplicationDbContext context)
        {
            _context = context;
        }
		public IActionResult AboutUs()
        {
            List<Team> teams = _context.Teams.ToList();
            return View(teams);
        }

        public IActionResult FrontViewProduct(int? categoryId)
        {
			// Get all categories
			List<Category> categories = _context.Categories.ToList();

			// If a category ID is specified, filter products by category, otherwise get all products
			List<Product> products = categoryId.HasValue
				? _context.Products.Where(p => p.CategoryId == categoryId.Value && p.IsActive).ToList()
				: _context.Products.Where(p => p.IsActive).ToList();

            // Create a ViewModel and assign products and categories
            var viewModel = new ProductCategoryViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategoryId = categoryId
            };

			return View(viewModel);
        }

        public IActionResult CheckLogin()
        {
            bool isLoggedIn = User.Identity.IsAuthenticated;
            return Json(isLoggedIn);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(int productId)
        {
            // Get the current user logged-in 
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product and user is exist
            var product = _context.Products.Find(productId);
            var user = _context.Users.Find(userId);

            if (product == null || user == null)
            {
                return Json(new { success = false, message = "Product or user not found." });
            }

            // Check if the product is already in the user's cart
            var existingCartItem = _context.Carts
                .Where(c => c.ProductId == productId && c.UserId == userId)
                .FirstOrDefault();

            if(existingCartItem != null)
            {
                // Product already in your cart
                existingCartItem.Quantity++;
                return Json(new { success = true, message = "Product alreaady in your cart." });
            }
            else
            {
                // If not, add the product to the cart
                var newCartItem = new Cart
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = 1
                };

                _context.Carts.Add(newCartItem);
            }

            
            _context.SaveChanges();

            return Json(new { success = true, message = "Product added to cart successfully." });
        }

        [HttpGet]
        public IActionResult ProductDetail(int productId)
        {
            // Retrieve product details from the database based on id
            var product = _context.Products
                .Include(p => p.Category).FirstOrDefault(p => p.ProductId == productId);

            if(product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        //[Authorize]
        public IActionResult IsProductInCart(int productId)
        {
            // Get the current user logged-in 
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product is in the user's cart
            var isInCart = _context.Carts
                .Any(c => c.ProductId == productId && c.UserId == userId);

            return Json(isInCart);
        }


        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromCart(int productId)
        {
            // Get the current user logged-in 
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product and user exist
            var product = _context.Products.Find(productId);
            var user = _context.Users.Find(userId);

            if (product == null || user == null)
            {
                return Json(new { success = false, message = "Product or user not found." });
            }

            // Check if the product is in the user's cart
            var existingCartItem = _context.Carts
                .Where(c => c.ProductId == productId && c.UserId == userId)
                .FirstOrDefault();

            if (existingCartItem != null)
            {
                // If the product is in the cart, remove it
                _context.Carts.Remove(existingCartItem);
                _context.SaveChanges();

                return Json(new { success = true, message = "Product removed from your cart." });
            }
            else
            {
                // Product is not in the cart
                return Json(new { success = false, message = "Product is not in your cart." });
            }
        }


        [Authorize]
        public IActionResult CountCarts()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the count of products in the user's cart
            int cartItemCount = _context.Carts
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            return Json(new { CartItemCount = cartItemCount });

        }

        public IActionResult GetCartProducts()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Assuming you have an exchange rate (e.g., 1 USD = 4100 KHR)
            double exchangeRate = 4100;

            // Get the cart items for the user
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new CartViewModel
                {
                    ProductId = c.Product.ProductId,
                    ImageUrl = c.Product.ImageUrl,
                    Name = c.Product.Name,
                    Description = c.Product.Description,
                    FormattedPrice = $"{c.Product.Price:C}", // Format price as currency
                    Quantity = c.Quantity,
                    FormattedTotal = $"{c.Quantity * c.Product.Price:C}", // Format total as currency
                    KhmerPrice = c.Product.Price * exchangeRate,
                    USDPrice = c.Product.Price,
                })
                .ToList();
            
            return View(cartItems);
        }

        [HttpPost]
        public ActionResult UpdateCartTotal()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Assuming you have an exchange rate (e.g., 1 USD = 4100 KHR)
                double exchangeRate = 4100;

                // Perform logic to update the cart total based on the updated quantities
                double updatedTotalUSD = 0;
                double updatedTotalKHR = 0;

                // Get the cart items for the user
                var cartItems = _context.Carts
                    .Where(c => c.UserId == userId)
                    .Select(c => new CartViewModel
                    {
                        ImageUrl = c.Product.ImageUrl,
                        Name = c.Product.Name,
                        Description = c.Product.Description,
                        FormattedPrice = $"{c.Product.Price:C}", // Format price as currency
                        Quantity = c.Quantity,
                        FormattedTotal = $"{c.Quantity * c.Product.Price:C}", // Format total as currency
                        KhmerPrice = c.Product.Price * exchangeRate,
                        USDPrice = c.Product.Price,
                    })
                    .ToList();

                

                foreach (var cartItem in cartItems)
                {
                    // Calculate total based on the provided properties
                    updatedTotalUSD += cartItem.Quantity * cartItem.USDPrice; // Assuming Price is in USD
                    updatedTotalKHR += cartItem.Quantity * cartItem.KhmerPrice; // Assuming KhmerPrice is in KHR
                }

                // Return the updated total prices
                return Json(new { totalUSD = updatedTotalUSD, totalKHR = updatedTotalKHR });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { error = "An error occurred while updating the cart total." });
            }
        }

        [HttpGet]
        public IActionResult GetFilteredProducts(int categoryId)
        {
            List<Product> products = _context.Products.Where(p => p.CategoryId == categoryId).ToList();
            return Json(products);
        }

        [HttpGet]
        public IActionResult GetMoreProducts(int categoryId, int totalLoaded)
        {
            // Fetch the next batch of products based on category and totalLoaded
            List<Product> products = _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Skip(totalLoaded)
                .Take(4)
                .ToList();

            return Json(products);
        }
    }
}
