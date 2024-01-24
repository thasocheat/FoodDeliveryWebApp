using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.Repository;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    //[Authorize]
    //[Route("Category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public CategoryController(ICategoryRepository categoryRepository, IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _categoryRepository = categoryRepository;
            _webHost = webHost;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> category = await _categoryRepository.GetAll();
            return View(category);
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetAll().Result;
            return Json(categories);
        }

        [HttpGet]
        public IActionResult GetbyId(int id)
        {
            try
            {
                var team = _categoryRepository.GetByIdAsync(id).Result;
                if (team == null)
                {
                    return NotFound(new { error = "Category not found" });
                }
                return Json(team);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error respone
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetbyId(int id, Category updatedCategory)
        {
            try
            {
                // Check if the provided ID matches any existing team
                var existingCategory = await _categoryRepository.GetByIdAsync(id);

                if (existingCategory == null)
                {
                    return NotFound(new { error = "Category not found" });
                }

                // Update the properties of the existing team with the new values
                existingCategory.CategoryName = updatedCategory.CategoryName;
                existingCategory.IsActive = updatedCategory.IsActive;
                existingCategory.CreatedAt = updatedCategory.CreatedAt;
                // Update other properties as needed

                // Process the updated image only if a new image is provided
                if (updatedCategory.ImageFile != null && updatedCategory.ImageFile.Length > 0)
                {
                    // Delete the existing image file
                    DeleteImageFile(existingCategory.ImageUrl);

                    string uniqueFileName = GetProfilePhotoFileName(updatedCategory);
                    existingCategory.ImageUrl = uniqueFileName;
                }

                // Save the changes to the database
                _categoryRepository.Update(existingCategory);
                _categoryRepository.Save();

                return Json(existingCategory);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }



        [HttpPost]
        public IActionResult Create(Category newCategory)
        {
            try
            {
                
                // Process the uploaded image only if it's provided
                if (newCategory.ImageFile != null && newCategory.ImageFile.Length > 0)
                {
                    string uniqueFileName = GetProfilePhotoFileName(newCategory);
                    newCategory.ImageUrl = uniqueFileName;
                }
                // Save the new team
                _categoryRepository.Add(newCategory);
                _categoryRepository.Save();

                return Json(newCategory);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, Category updatedCategory)
        {
            try
            {
                // Check if the provided ID matches any existing team
                var existingCategory = await _categoryRepository.GetByIdAsync(id);

                if (existingCategory == null)
                {
                    return NotFound(new { error = "Category not found" });
                }

                // Update only IsActive
                //existingCategory.IsActive = updatedCategory.IsActive;
                existingCategory.IsActive = !existingCategory.IsActive;
                // Update other properties as needed                

                // Save the changes to the database
                _categoryRepository.Update(existingCategory);
                _categoryRepository.Save();

                return Json(existingCategory);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                // Return a JSON result for success
                return Json(new { success = true, message = "Category data not found!" });
            }

            return View(category);
        }


        private void SaveNewImage(IFormFile imageFile, string uniqueFileName)
        {
            // Ensure _webHost.WebRootPath is not null
            if (!string.IsNullOrEmpty(_webHost.WebRootPath))
            {
                // Combine the web root path with the "Images/Teams" folder
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Categories");

                // Combine the uploads folder with the unique file name
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the "wwwroot/Images/Teams" folder
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
            }
        }



        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _categoryRepository.GetById(id);

            if (category == null)
            {
                // Return a JSON result for not found
                return Json(new { success = false, message = "Category not found" });
            }

            // Delete the image file
            DeleteImageFile(category.ImageUrl);

            // Delete the team from the database
            _categoryRepository.Delete(category);

            // Return a JSON result for success
            return Json(new { success = true, message = "Category deleted successfully" });
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

        public string GetProfilePhotoFileName(Category category)
        {
            string uniqueFileName = null;

            // Process the uploaded image only if it's provided
            if (category.ImageFile != null && category.ImageFile.Length > 0)
            {
                // Ensure _webHost.WebRootPath is not null
                if (!string.IsNullOrEmpty(_webHost.WebRootPath))
                {
                    // Combine the web root path with the "Images/Teams" folder
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Categories");

                    // Generate a unique file name
                    string fileName = Path.GetFileName(category.ImageFile.FileName);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(category.ImageFile.FileName);

                    // Combine the uploads folder with the unique file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the uploaded file to the "wwwroot/Images/Teams" folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        category.ImageFile.CopyTo(fileStream);
                    }

                    // Include the relative path in the ImageUrl
                    return $"/Images/Categories/{uniqueFileName}";
                }
            }

            return uniqueFileName;
        }
    }
}
