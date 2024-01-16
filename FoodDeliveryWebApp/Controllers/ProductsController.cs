using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
