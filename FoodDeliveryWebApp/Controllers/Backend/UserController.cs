using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.Repository;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public UserController(IUserRepository userRepository, IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _webHost = webHost;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<AppUser> users = await _userRepository.GetAll();
            return View(users);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.GetAll().Result;
            return Json(users);
        }

        [HttpPost]
        public IActionResult Create(UserViewModel newUserViewModel)
        {
            try
            {
                // Convert UserViewModel to AppUser
                var newUser = new AppUser
                {
                    // Map other properties as needed
                    UserName = newUserViewModel.UserName,
                    Phone = newUserViewModel.Phone,
                    Address = newUserViewModel.Address,
                    Email = newUserViewModel.Email,
                    CreateAt = newUserViewModel.CreateAt,
                    // ... map other properties ...

                    // Set the password using ASP.NET Core Identity's password hasher
                    PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, newUserViewModel.Password)
                };

                // Process the uploaded image only if it's provided
                if (newUserViewModel.ImageFile != null && newUserViewModel.ImageFile.Length > 0)
                {
                    string uniqueFileName = GetProfilePhotoFileName(newUserViewModel);
                    newUser.ImageUrl = uniqueFileName;
                }

                // Save the new user
                _userRepository.Add(newUser);
                _userRepository.Save();

                return Json(newUser);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }



        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                var user = _userRepository.GetById(id).Result;
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                //var userDto = new UserViewModel
                //{
                //    Id = user.Id,
                //    UserName = user.UserName,

                //};

                //var serializedUser = JsonConvert.SerializeObject(userDto);
                //return Content(serializedUser, "application/json");
                return Json(user);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error respone
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

       


        [HttpPost]
        public async Task<IActionResult> Update(string id, UserViewModel updatedUser)
        {
            try
            {
                // Check if the provided ID matches any existing team
                var existingUser = await _userRepository.GetById(id);

                if (existingUser == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                // Update the properties of the existing team with the new values
                existingUser.UserName = updatedUser.UserName;
                existingUser.Phone = updatedUser.Phone;
                existingUser.Email = updatedUser.Email;
                // Check if a new password is provided
                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    // Set the new password using ASP.NET Core Identity's password hasher
                    existingUser.PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, updatedUser.Password);
                }
                //else
                //{
                //    existingUser.PasswordHash = existingUser.PasswordHash;
                //}
                existingUser.Address = updatedUser.Address;
                
                // Update date if a new data is provided
                if(updatedUser.CreateAt != null)
                {
                    existingUser.CreateAt = updatedUser.CreateAt;
                }
                // Update other properties as needed

                // Process the updated image only if a new image is provided
                if (updatedUser.ImageFile != null && updatedUser.ImageFile.Length > 0)
                {
                    // Delete the existing image file
                    DeleteImageFile(existingUser.ImageUrl);

                    string uniqueFileName = GetProfilePhotoFileName(updatedUser);
                    existingUser.ImageUrl = uniqueFileName;
                }

                // Save the changes to the database
                _userRepository.Update(existingUser);
                _userRepository.Save();

                return Json(existingUser);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }



        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userRepository.GetById(id);

            if (user == null)
            {
                // Return a JSON result for not found
                return Json(new { success = false, message = "User not found" });
            }

            // Delete the image file
            DeleteImageFile(user.ImageUrl);

            // Delete the team from the database
            _userRepository.Delete(user);

            // Return a JSON result for success
            return Json(new { success = true, message = "User deleted successfully" });
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


    }
}
