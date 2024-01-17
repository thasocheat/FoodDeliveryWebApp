using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Data
{
    public class ProductSeed
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if there are existing products
                if (context.Products.Any())
                {
                    return; // Database has been seeded
                }

                // Seed Products
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Rice",
                        Description = "White rice",
                        Price = 10,
                        Quantity = 15,
                        ImageUrl = "Images/Products/rice.png",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CategoryId = 1
                    },
                    new Product
                    {
                        Name = "Orange Juice",
                        Description = "New oraange juice",
                        Price = 5,
                        Quantity = 5,
                        ImageUrl = "Images/Products/orange-juice.png",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CategoryId = 2
                    },
                };
                context.Products.AddRange(products);
                context.SaveChanges();

            }
        }
    }
}
