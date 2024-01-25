using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.Models
{
    public class AppUser : IdentityUser
    {        
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; } // Image file to upload during creation
        [NotMapped]
        [Display(Name = "Ne Upload Image")]
        public IFormFile NewImage { get; set; } // New image file to upload during update
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }

        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
