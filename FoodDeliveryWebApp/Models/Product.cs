using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryWebApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Order> Orders { get; set;}
        public ICollection<Cart> Carts { get; set; }
    }
}
