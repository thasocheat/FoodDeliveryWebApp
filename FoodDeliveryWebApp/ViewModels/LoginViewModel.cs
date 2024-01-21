using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebApp.ViewModels
{
    public class LoginViewModel
    {

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
