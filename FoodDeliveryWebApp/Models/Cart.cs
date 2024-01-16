using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("Porduct")]
        public int? ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("AppUser")]
        public int? UserId { get; set; }

        public Product Product { get; set; }
        public AppUser AppUser { get; set; }
    }
}
