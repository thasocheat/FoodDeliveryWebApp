using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; } // Image file to upload during creation
        [NotMapped]
        [Display(Name = "Ne Upload Image")]
        public IFormFile NewImage { get; set; } // New image file to upload during update
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Old Password")]
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }

        public List<OrderViewModel> OrderHistory { get; set; }
    }
}
