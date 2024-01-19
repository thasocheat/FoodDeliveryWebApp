using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Data
{
    public class CategorySeed
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if there are existing categoryies
                if (context.Categories.Any())
                {
                    return; // Database has been seeded
                }

                // Seed Categories
                var categories = new List<Category>
                {
                    new Category
                    {
                        CategoryName = "Food",
                        ImageUrl = "/Images/Ctegories/diet.png",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        Products = new List<Product>()
                    },
                    new Category
                    {
                        CategoryName = "Drink",
                        ImageUrl = "/Images/Ctegories/drink.png",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        Products = new List<Product>()
                    },
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();

            }
        }
    }
}
