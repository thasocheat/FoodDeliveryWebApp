using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net;

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



        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Staff))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "teddysmithdeveloper@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "teddysmithdev",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        Address = "Siem Reap"
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appStaffEmail = "staff@worker.com";

                var appStaff = await userManager.FindByEmailAsync(appStaffEmail);
                if (appStaff == null)
                {
                    var newAppStaff = new AppUser()
                    {
                        UserName = "app-staff",
                        Email = appStaffEmail,
                        EmailConfirmed = true,
                        Address = "Siem Reap"
                    };
                    await userManager.CreateAsync(newAppStaff, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppStaff, UserRoles.Staff);
                }

                string appUserEmail = "user@etickets.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        Address = "Siem Reap"
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
            }
        }



    }
}
