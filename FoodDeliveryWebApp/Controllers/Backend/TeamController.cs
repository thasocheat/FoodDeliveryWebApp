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
        
        
        public async Task<IActionResult> Listing(int id)
		{
            Team team = await _teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                // Return a JSON result for not found
                return Json(new { success = false, message = "Team not found" + id });
            }

            return Json(new { success = true, message = "Team listing successfully" });

        }





        public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Team team)
		{
			

			if (!ModelState.IsValid)
			{
				return View(team);
			}
            
            // Process the uploaded image only if it's provided
            if(team.ImageFile != null && team.ImageFile.Length > 0)
            {
                string uniqueFileName = GetProfilePhotoFileName(team);
                team.ImageUrl = uniqueFileName;
            }
            _teamRepository.Add(team);
            TempData["SweetAlert"] = "Successfully Created";
            return RedirectToAction("Index");
		}

        public async Task<IActionResult> Edit(int id)
        {
            Team team = await _teamRepository.GetByIdAsync(id);

            if (team == null)
            {
                // Display a SweetAlert for not found
                TempData["SweetAlert"] = "NotFound";
                return RedirectToAction("Index");
            }

            return View(team);
        }

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
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Images/Teams");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(team.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the wwwroot/Images/Teams folder
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    team.ImageFile.CopyTo(fileStream);
                }
                // Include the relative path in the ImageUrl
                if (!string.IsNullOrEmpty(uniqueFileName))
                {
                    return $"/Images/Teams/{uniqueFileName}";
                }
            }

            return uniqueFileName;
        }

		
	}
}
