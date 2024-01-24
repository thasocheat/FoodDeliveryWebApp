using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{
	//[Authorize]
	public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;

        public TeamController(ITeamRepository teamRepository, IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _teamRepository = teamRepository;
            _webHost = webHost;
            _context = context;
        }

		public async Task<IActionResult> Index()
		{
			IEnumerable<Team> teams = await _teamRepository.GetAll();
			return View(teams);
		}


        [HttpGet]
        public IActionResult GetTeams()
        {
            var teams = _teamRepository.GetAll().Result;
            return Json(teams);
        }

        [HttpGet]
        public IActionResult GetbyId(int id)
        {
            try
            {
                var team = _teamRepository.GetByIdAsync(id).Result;
                if(team == null)
                {
                    return NotFound(new { error = "Team not found" });
                }
                return Json(team);
            }
            catch(Exception ex)
            {
                // Log the exception and return an error respone
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetbyId(int id, Team updatedTeam)
        {
            try
            {
                // Check if the provided ID matches any existing team
                var existingTeam = await _teamRepository.GetByIdAsync(id);

                if (existingTeam == null)
                {
                    return NotFound(new { error = "Team not found" });
                }

                // Update the properties of the existing team with the new values
                existingTeam.Name = updatedTeam.Name;
                existingTeam.Email = updatedTeam.Email;
                existingTeam.Description = updatedTeam.Description;
                existingTeam.Title = updatedTeam.Title;
                existingTeam.Bio = updatedTeam.Bio;
                existingTeam.CreatedAt = updatedTeam.CreatedAt;
                // Update other properties as needed

                // Process the updated image only if a new image is provided
                if (updatedTeam.ImageFile != null && updatedTeam.ImageFile.Length > 0)
                {
                    // Delete the existing image file
                    DeleteImageFile(existingTeam.ImageUrl);

                    string uniqueFileName = GetProfilePhotoFileName(updatedTeam);
                    existingTeam.ImageUrl = uniqueFileName;
                }

                // Save the changes to the database
                _teamRepository.Update(existingTeam);
                _teamRepository.Save();

                return Json(existingTeam);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Log the exception details
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }



        [HttpPost]
        public IActionResult Create(Team newTeam)
        {
            try
            {
                //// Save the new image
                //newTeam.ImageUrl = SaveNewImage(newTeam.ImageFile);
                // Process the uploaded image only if it's provided
                if (newTeam.ImageFile != null && newTeam.ImageFile.Length > 0)
                {
                    string uniqueFileName = GetProfilePhotoFileName(newTeam);
                    newTeam.ImageUrl = uniqueFileName;
                }
                // Save the new team
                _teamRepository.Add(newTeam);
                _teamRepository.Save();

                return Json(newTeam);
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
            var team = await _teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                // Return a JSON result for success
                return Json(new { success = true, message = "Team data not found!" });
            }

            return View(team);
        }


        private void SaveNewImage(IFormFile imageFile, string uniqueFileName)
        {
            // Ensure _webHost.WebRootPath is not null
            if (!string.IsNullOrEmpty(_webHost.WebRootPath))
            {
                // Combine the web root path with the "Images/Teams" folder
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Teams");

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
           Team team = await _teamRepository.GetById(id);

            if (team == null)
            {
                // Return a JSON result for not found
                return Json(new { success = false, message = "Team not found" });
            }

            // Delete the image file
            DeleteImageFile(team.ImageUrl);

            // Delete the team from the database
            _teamRepository.Delete(team);

            // Return a JSON result for success
            return Json(new { success = true, message = "Team deleted successfully" });
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

        public string GetProfilePhotoFileName(Team team)
        {
            string uniqueFileName = null;

            // Process the uploaded image only if it's provided
            if (team.ImageFile != null && team.ImageFile.Length > 0)
            {
                // Ensure _webHost.WebRootPath is not null
                if (!string.IsNullOrEmpty(_webHost.WebRootPath))
                {
                    // Combine the web root path with the "Images/Teams" folder
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Teams");

                    // Generate a unique file name
                    string fileName = Path.GetFileName(team.ImageFile.FileName);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(team.ImageFile.FileName);

                    // Combine the uploads folder with the unique file name
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the uploaded file to the "wwwroot/Images/Teams" folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        team.ImageFile.CopyTo(fileStream);
                    }

                    // Include the relative path in the ImageUrl
                    return $"/Images/Teams/{uniqueFileName}";
                }
        }

            return uniqueFileName;
        }

        



    }
}
