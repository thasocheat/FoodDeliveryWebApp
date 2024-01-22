using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
