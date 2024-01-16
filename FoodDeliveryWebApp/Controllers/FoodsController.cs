using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers
{
    public class FoodsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
