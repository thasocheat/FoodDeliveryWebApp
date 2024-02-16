using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Frontend
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
