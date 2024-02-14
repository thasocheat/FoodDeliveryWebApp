using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Auth
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _signInManager = signInManager;
            _webHost = webHost;
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
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    // return RedirectToAction();

                    // Check user's credentials
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return Json(new { success = true, message = "Admin login successful", redirectUrl = Url.Action("Index", "Dashboard") });
                    } else if (roles.Contains("Staff"))
                    {
                        return Json(new { success = true, message = "Staff login successful", redirectUrl = Url.Action("Index", "Dashboard") });

                    }
                    else
                    {
                        return Json(new { success = true, message = "Login successful", redirectUrl = Url.Action("Index", "Home") });

                    }
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

        [HttpGet]
        public async Task<IActionResult> ProfileUser()
        {
            // Get the currently logged-id user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Redirect to login page if the user is not logged in
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user's order history
            var orderHistory = _context.Orders
                .Where(o => o.UserId == currentUser.Id)
                .GroupBy(o => new { o.OrderNo, o.PaymentId })
                .Select(group => new OrderViewModel
                {
                    OrderId = group.FirstOrDefault().OrderId,
                    OrderAt = group.First().OrderAt,
                    Price = group.First().Product.Price,
                    Status = group.First().Status,

                    // Calculate total quantity and total amount form the order goup
                    TotalQuantity = group.Sum(o => o.Quantity),
                    TotalAmount = group.Sum(o => o.Quantity * o.Product.Price),

                    Quantity = group.Sum(o => o.Quantity),
                    OrderNo = group.Key.OrderNo,

                    OrderItems = _context.Orders
                    .Where(oi => oi.OrderId == group.FirstOrDefault().OrderId)
                    .Select(oi => new OrderItemViewModel
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        ProductImageUrl = oi.Product.ImageUrl,
                        ProductPrice = oi.Product.Price,
                        Quantity = oi.Quantity,
                        TotalQuantity = group.Sum (o => o.Quantity),
                        TotalAmount = group.Sum(o => o.Quantity * o.Product.Price),
                    }).ToList()

                }).ToList();

            // Get the user data and order history
            var userViewModel = new UserViewModel
            {
                UserName = currentUser.UserName,
                Phone = currentUser.Phone,
                Email = currentUser.Email,
                Address = currentUser.Address,
                CreateAt = currentUser.CreateAt,
                ImageUrl = currentUser.ImageUrl,
                OrderHistory = orderHistory
            };

            return View(userViewModel);
        }

        //[HttpGet]
        //public IActionResult GetUserDetails()
        //{
        //    try
        //    {
        //        // Retrieve user details including payment information based on the authenticated user
        //        string userId = User.Identity.Name;

        //        // Retrieve user details from the database
        //        var userDetails = _context.Users
        //            .Where(u => u.Id == userId)
        //            .Select(u => new
        //            {
        //                u.Name,
        //                u.CardNo,         // Adjust property names based on your User model
        //                u.ExpiryDate,
        //                u.CvvNo,
        //                u.Address,
        //                u.PaymentMode
        //                // Add other user-related properties as needed
        //            })
        //            .FirstOrDefault();

        //        if (userDetails == null)
        //        {
        //            return Json(new { success = false, message = "User details not found" });
        //        }

        //        return Json(new { success = true, userDetails });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for debugging purposes
        //        Console.Error.WriteLine(ex);

        //        // Handle exceptions
        //        return Json(new { success = false, message = "Error retrieving user details" });
        //    }
        //}



        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UserViewModel model)
        {
            try
            {
                // Get the current user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    // User not found
                    return Json(new { success = false, message = "User not found" });
                }

                // Verify the old password
                var result = await _userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    // Password updated successfully
                    await _signInManager.SignInAsync(currentUser, isPersistent: false);
                    return Json(new { success = true, message = "Password updated successfully" });
                }
                else
                {
                    // Password update failed
                    return Json(new { success = false, message = "Invalid old password or password update failed" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return Json(new { success = false, error = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileAjax(UserViewModel updatedProfile)
        {
            try
            {
                // Get the currently logged-in user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    // Redirect to login page or handle the case where the user is not logged in
                    return Json(new { success = false, message = "User not found" });
                }

                // Update the properties of the current user with the new values
                currentUser.UserName = updatedProfile.UserName;
                currentUser.Phone = updatedProfile.Phone;
                currentUser.Email = updatedProfile.Email;
                currentUser.Address = updatedProfile.Address;
                currentUser.CreateAt = updatedProfile.CreateAt;
                // Update other properties as needed

                // Process the updated image only if a new image is provided
                if (updatedProfile.ImageFile != null && updatedProfile.ImageFile.Length > 0)
                {
                    // Delete the existing image file
                    DeleteImageFile(currentUser.ImageUrl);

                    string uniqueFileName = GetProfilePhotoFileName(updatedProfile);
                    currentUser.ImageUrl = uniqueFileName;
                }

                // Save the changes to the database
                await _userManager.UpdateAsync(currentUser);

                return Json(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return Json(new { success = false, error = "Internal Server Error" });
            }
        }

        private void DeleteImageFile(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string imagePath = Path.Combine(_webHost.WebRootPath, imageUrl.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }

        public string GetProfilePhotoFileName(UserViewModel user)
        {
            string uniqueFileName = null;

            // Process the uploaded image only if it's provided
            if (user.ImageFile != null && user.ImageFile.Length > 0)
            {
                // Ensure _webHost.WebRootPath is not null
                if (!string.IsNullOrEmpty(_webHost.WebRootPath))
                {
                    // Combine the web root path with the "Images/Teams" folder
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Users");

                    // Generate a unique file name
                    string fileName = Path.GetFileName(user.ImageFile.FileName);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(user.ImageFile.FileName);

                    // Combine the uploads folder with the unique file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the uploaded file to the "wwwroot/Images/Teams" folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        user.ImageFile.CopyTo(fileStream);
                    }

                    // Include the relative path in the ImageUrl
                    return $"/Images/Users/{uniqueFileName}";
                }
            }

            return uniqueFileName;
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
