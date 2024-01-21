using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Auth
{    
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            // To hold the value
            var response = new LoginViewModel();
            return View(response);
        }

        //[HttpPost]
        //public IActionResult CheckUser([FromBody] LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Check if the user with the provided email exists in the database
        //        var user = _userManager.FindByEmailAsync(model.Email).Result;

        //        if (user != null && _userManager.CheckPasswordAsync(user, model.Password).Result)
        //        {
        //            return Json(new { success = true, message = "User exists" });
        //        }
        //    }

        //    return Json(new { success = false, message = "Invalid email or password" });
        //}

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            // Check
            if (!ModelState.IsValid)
            {
                //return Json(loginVM);
                return Json(new { success = false, message = "Invalid form submission" });
            }

            // Get user id
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    // return RedirectToAction();
                    return Json(new { success = true, message = "Login successful", redirectUrl = Url.Action("Index", "Home") });
                }

                if (result.IsLockedOut)
                {
                    return Json(new { success = false, message = "Account locked out" });
                }
                else if (result.IsNotAllowed)
                {
                    return Json(new { success = false, message = "Login nor allowed" });
                }
                else if (result.RequiresTwoFactor)
                {
                    return Json(new { success = false, message = "Two-factor authentication required" });
                }
                else
                {
                    // If password is incorrect
                    //return Json(loginVM);
                    return Json(new { success = false, message = "Invalid login attempt" });
                }
            }

            // User not found
            //return Json(loginVM);
            return Json(new { success = false, message = "User not found" });


        }



        public IActionResult Register()
        {
            // To hold the value
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            // Check form submission validity
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form submission" });
            }

            try
            {
                // Check if the email already exists in the database
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "This email is already registered. Please use a different email address." });
                }

                // Create a new user
                var newUser = new AppUser
                {
                    UserName = registerVM.Email,
                    Email = registerVM.Email,
                    // Add any additional properties from the registration form
                };

                // Attempt to create the user
                var result = await _userManager.CreateAsync(newUser, registerVM.Password);

                if (result.Succeeded)
                {
                    // Assign a default role for all registered users
                    var roles = new List<string> { "User" };
                    await _userManager.AddToRolesAsync(newUser, roles);

                    // Sign in the newly registered user
                    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    return Json(new { success = true, message = "Registration successful", redirectUrl = Url.Action("Index", "Home") });
                }

                // Handle registration failure scenarios
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Json(new { success = false, message = "Registration failed", errors = result.Errors.Select(error => error.Description) });
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                // Logger.LogError(ex, "An error occurred during user registration");
                return Json(new { success = false, message = "An unexpected error occurred during registration" });
            }
        }




        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Json(new { success = true, message = "Logout successful", redirectUrl = Url.Action("Index", "Home") });
        }




    }
}
