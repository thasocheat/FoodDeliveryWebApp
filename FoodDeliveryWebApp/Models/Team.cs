using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; } // Image file to upload during creation
        [NotMapped]
        [Display(Name = "Ne Upload Image")]
        public IFormFile NewImage { get; set; } // New image file to upload during update
        public string? ImageUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
