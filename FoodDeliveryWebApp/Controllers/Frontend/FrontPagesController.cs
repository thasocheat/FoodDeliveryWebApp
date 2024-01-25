using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            return View();
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
