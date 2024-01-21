using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
