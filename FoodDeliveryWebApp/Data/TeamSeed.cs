using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Data
{
    public class TeamSeed
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if there are existing teams
                if (context.Teams.Any())
                {
                    return; // Database has been seeded
                }

                // Seed Teams
                var teams = new List<Team>
                {
                    new Team
                    {
                        Name = "Soeung Lysan",
                        Email = "devteam@example.com",
                        Description = "A team of skilled developers",
                        Title = "Development Team Lead",
                        ImageUrl = "/Images/Teams/lysun.jpg",
                        Bio = "Our development team is dedicated to building high-quality software.",
                        CreatedAt = DateTime.Now
                    },
                    new Team
                    {
                        Name = "Po Rozel",
                        Email = "devteam@example.com",
                        Description = "A team of skilled developers",
                        Title = "Development Team Lead",
                        ImageUrl = "/Images/Teams/rozel.jpg",
                        Bio = "Our development team is dedicated to building high-quality software.",
                        CreatedAt = DateTime.Now
                    },
                    new Team
                    {
                        Name = "Tha Socheat",
                        Email = "devteam@example.com",
                        Description = "A team of skilled developers",
                        Title = "Development Team Lead",
                        ImageUrl = "/Images/Teams/socheat.jpg",
                        Bio = "Our development team is dedicated to building high-quality software.",
                        CreatedAt = DateTime.Now
                    },
                    new Team
                    {
                        Name = "Han Wakim",
                        Email = "devteam@example.com",
                        Description = "A team of skilled developers",
                        Title = "Development Team Lead",
                        ImageUrl = "/Images/Teams/khim.jpg",
                        Bio = "Our development team is dedicated to building high-quality software.",
                        CreatedAt = DateTime.Now
                    },
                    new Team
                    {
                        Name = "Rom Heng",
                        Email = "devteam@example.com",
                        Description = "A team of skilled developers",
                        Title = "Development Team Lead",
                        ImageUrl = "/Images/Teams/heng.jpg",
                        Bio = "Our development team is dedicated to building high-quality software.",
                        CreatedAt = DateTime.Now
                    },

                };
                context.Teams.AddRange(teams);
                context.SaveChanges();

            }
        }
    }
}
