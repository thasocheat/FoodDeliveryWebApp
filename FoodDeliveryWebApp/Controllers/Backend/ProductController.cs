using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Detail(int id)
        {

            Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            return View(product);
        }
    }
}
