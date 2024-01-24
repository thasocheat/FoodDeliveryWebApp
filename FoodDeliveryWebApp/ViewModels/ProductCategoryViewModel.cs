using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.ViewModels
{
    public class ProductCategoryViewModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        public int? SelectedCategoryId { get; set; }

        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
