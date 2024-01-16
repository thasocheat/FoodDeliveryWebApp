using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryWebApp.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
