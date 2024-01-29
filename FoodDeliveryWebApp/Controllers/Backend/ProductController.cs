using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.Repository;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public ProductController(ICategoryRepository categoryRepository, IProductRepository productRepository, IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _webHost = webHost;
            _context = context;
        }
        
        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
            List<Product> products = _context.Products.ToList();
            var models = new ProductCategoryViewModel
            {
                Products = products,
                Categories = categories,
            };
            return View(models);
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetAll().Result;
            return Json(products);
        }



        public IActionResult Detail(int id)
        {

            Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            return View(product);
        }

        [HttpGet]
        public IActionResult GetbyId(int id)
        {
            try
            {
                var product = _productRepository.GetByIdAsync(id).Result;
                if (product == null)
                {
                    return NotFound(new { error = "Product not found" });
                }
                return Json(product);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error respone
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost] // Used to be update
        public async Task<IActionResult> GetbyId(int id, Product updatedProduct)
        {
            try
            {
                // Check if the provided ID matches any existing product
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    return NotFound(new { error = "Product not found" });
                }

                // Update the properties of the existing product with the new values
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Quantity = updatedProduct.Quantity;
                existingProduct.IsActive = updatedProduct.IsActive;
                existingProduct.CreatedAt = updatedProduct.CreatedAt;
                existingProduct.CategoryId = updatedProduct.CategoryId;
                // Update other properties as needed

                // Process the updated image only if a new image is provided
                if (updatedProduct.ImageFile != null && updatedProduct.ImageFile.Length > 0)
                {
                    // Delete the existing image file
                    DeleteImageFile(existingProduct.ImageUrl);

                    string uniqueFileName = GetProfilePhotoFileName(updatedProduct);
                    existingProduct.ImageUrl = uniqueFileName;
                }

                // Save the changes to the database
                _productRepository.Update(existingProduct);
                _productRepository.Save();

                return Json(existingProduct);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }



        [HttpPost]
        public IActionResult Create(Product newProduct)
        {
            try
            {

                // Process the uploaded image only if it's provided
                if (newProduct.ImageFile != null && newProduct.ImageFile.Length > 0)
                {
                    string uniqueFileName = GetProfilePhotoFileName(newProduct);
                    newProduct.ImageUrl = uniqueFileName;
                }

                // Get the category and save for product
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == newProduct.CategoryId);
                if (category == null)
                {
                    return Json(new { error = "Category not found!" });
                }
                // Set the category property of the new product
                newProduct.Category = category;

                // Save the new product
                _productRepository.Add(newProduct);
                _productRepository.Save();

                return Json(newProduct);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, Product updatedProduct)
        {
            try
            {
                // Check if the provided ID matches any existing product
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    return NotFound(new { error = "Product not found" });
                }

                // Update only IsActive
                existingProduct.IsActive = !existingProduct.IsActive;
                // Update other properties as needed                

                // Save the changes to the database
                _productRepository.Update(existingProduct);
                _productRepository.Save();

                return Json(existingProduct);
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
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                // Return a JSON result for success
                return Json(new { success = true, message = "Product data not found!" });
            }

            return View(product);
        }


        private void SaveNewImage(IFormFile imageFile, string uniqueFileName)
        {
            // Ensure _webHost.WebRootPath is not null
            if (!string.IsNullOrEmpty(_webHost.WebRootPath))
            {
                // Combine the web root path with the "Images/Teams" folder
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Products");

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
            Product product = await _productRepository.GetById(id);

            if (product == null)
            {
                // Return a JSON result for not found
                return Json(new { success = false, message = "Product not found" });
            }

            // Delete the image file
            DeleteImageFile(product.ImageUrl);

            // Delete the team from the database
            _productRepository.Delete(product);

            // Return a JSON result for success
            return Json(new { success = true, message = "Product deleted successfully" });
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

        public string GetProfilePhotoFileName(Product product)
        {
            string uniqueFileName = null;

            // Process the uploaded image only if it's provided
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                // Ensure _webHost.WebRootPath is not null
                if (!string.IsNullOrEmpty(_webHost.WebRootPath))
                {
                    // Combine the web root path with the "Images/Teams" folder
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Products");

                    // Generate a unique file name
                    string fileName = Path.GetFileName(product.ImageFile.FileName);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageFile.FileName);

                    // Combine the uploads folder with the unique file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the uploaded file to the "wwwroot/Images/Teams" folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        product.ImageFile.CopyTo(fileStream);
                    }

                    // Include the relative path in the ImageUrl
                    return $"/Images/Products/{uniqueFileName}";
                }
            }

            return uniqueFileName;
        }
    }
}
