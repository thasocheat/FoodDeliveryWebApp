using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{
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

        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(Team team)
        //{


        //    if (!ModelState.IsValid)
        //    {
        //        return View(team);
        //    }

    //        // Process the uploaded image only if it's provided
    //        if (team.ImageFile != null && team.ImageFile.Length > 0)
    //        {
    //            string uniqueFileName = GetProfilePhotoFileName(team);
    //    team.ImageUrl = uniqueFileName;
    //        }
    //_teamRepository.Add(team);

        //    // Save changes to the database
        //    //await _teamRepository.SaveAsync();
        //    return Json(team);
        //}

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
                // Display a SweetAlert for not found
                // Return a JSON result for success
                return Json(new { success = true, message = "Team data not found!" });
            }

            return View(team);
        }

        //[HttpPost]
        //public IActionResult Update(Team updateTeam)
        //{
        //    try
        //    {
        //        // Find the existing team in teh repository by teamId
        //        var existingTeam = _teamRepository.GetByIdAsync(updateTeam.TeamId);

        //        if (existingTeam == null)
        //        {
        //            return Json(new { success = false, message = "Team not found" });
        //        }

        //        // Check if a new image is provided
        //        if(updateTeam.ImageFile != null && updateTeam.ImageFile.Length > 0)
        //        {
        //            // Delete the old image
        //            DeleteImageFile(existingTeam.ImageUrl);

        //            // Generate a unique file name for the new image
        //            string uniqueFileName = GetProfilePhotoFileName(updateTeam);

        //            // Save the new image
        //            SaveNewImage(updateTeam.ImageFile, uniqueFileName);

        //            // Set the ImageUrl of the update team to the new image path
        //            existingTeam.ImageUrl = $"/Images/Teams/{uniqueFileName}";
        //        }

        //        // Save the updated team
        //        _teamRepository.Update(existingTeam);
        //        _teamRepository.Save();

        //        return Json(existingTeam);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and return an error response
        //        // Log the exception details
        //        return StatusCode(500, new { error = "Internal Server Error" });
        //    }
        //}


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

        //[HttpPost]
        //[ValidateAntiForgeryToken] // Include this attribute for security
        //public async Task<IActionResult> Edit(int id, Team updatedTeam)
        //{
        //    if (id != updatedTeam.TeamId)
        //    {
        //        // IDs don't match, return a bad request
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        // Validate the updated team data
        //        if (ModelState.IsValid)
        //        {
        //            // Retrieve the existing team from the database
        //            var existingTeam = await _teamRepository.GetByIdAsync(id);

        //            if (existingTeam == null)
        //            {
        //                // Team not found, return an appropriate response
        //                return NotFound("Team not found");
        //            }

        //            // Save changes to the database
        //            _teamRepository.Update(existingTeam);
        //            await _teamRepository.SaveAsync();

        //            // Redirect to a success page or return a JSON result
        //            return Json(new { success = true, message = "Team deleted successfully" }); // Assuming you have an Index action in a Team controller
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        return StatusCode(500, new { error = "Internal Server Error" });
        //    }

        //    // If ModelState is not valid, return to the edit view with validation errors
        //    return View(updatedTeam);
        //}









        [HttpPost]
        public async Task<IActionResult> Edit(Team team)
        {
            if (!ModelState.IsValid)
            {
                // Display a SweetAlert for validation errors
                TempData["SweetAlert"] = "ValidationErrors";
                return View(team);
            }

            Team existingTeam = await _teamRepository.GetByIdAsync(team.TeamId);

            if (existingTeam == null)
            {
                // Display a SweetAlert for not found
                TempData["SweetAlert"] = "NotFound";
                return RedirectToAction("Index");
            }

            // Process the uploaded image only if it's provided
            if (team.ImageFile != null && team.ImageFile.Length > 0)
            {
                // Delete the existing image file
                DeleteImageFile(existingTeam.ImageUrl);

                // Save the new image file
                string uniqueFileName = GetProfilePhotoFileName(team);
                team.ImageUrl = uniqueFileName;
            }

            _teamRepository.Update(team);

            // Display a SweetAlert for success
            TempData["SweetAlert"] = "EditSuccess";
            return RedirectToAction("Index");
        }


        //private string? SaveNewImage(IFormFile imageFile)
        //{
        //    // Process the uploaded image only if it's provided
        //    if (imageFile != null && imageFile.Length > 0)
        //    {
        //        // Generate a unique file name
        //        string uniqueFileName = GetProfilePhotoFileName((Team)imageFile);

        //        // Get the path where the image will be stored
        //        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "images");

        //        // Combine the upload path with the unique file name
        //        string filePath = Path.Combine(uploadPath, uniqueFileName);

        //        // Save the image file to the specified location
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            imageFile.CopyTo(fileStream);
        //        }

        //        // Return the unique file name, which can be stored in the database
        //        return uniqueFileName;
        //    }
        //    return null; // No new image provided
        //}


        //public string GetProfilePhotoFileName(Team team)
        //{
        //    string uniqueFileName = null;

        //    // Process the uploaded image only if it's provided
        //    if (team.ImageFile != null && team.ImageFile.Length > 0)
        //    {
        //        string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Teams");
        //        uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(team.ImageFile.FileName);
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        // Save the uploaded file to the wwwroot/Images/Teams folder
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            team.ImageFile.CopyTo(fileStream);
        //        }
        //        // Include the relative path in the ImageUrl
        //        if (!string.IsNullOrEmpty(uniqueFileName))
        //        {
        //            return $"/Images/Teams/{uniqueFileName}";
        //        }
        //    }

        //    return uniqueFileName;
        //}


    }
}
