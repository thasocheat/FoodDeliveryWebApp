﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Backend
{
	[Authorize]
	public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
