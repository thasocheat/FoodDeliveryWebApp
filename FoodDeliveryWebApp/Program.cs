using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services for repository
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add services for connecting to SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie();

var app = builder.Build();

// Run all seeder file for example user use
if(args.Length == 1 && args[0].ToLower() == "seeddata")
{
	//Seed.SeedData(app);
	await Seeds.SeedUsersAndRolesAsync(app);
	SeedData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
	name: "backend",
	pattern: "{area:exists}/{controller=Team}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
//pattern: "{controller=Dashboard}/{action=Index}/{id?}");


app.Run();


// Method to seed data
void SeedData(WebApplication app)
{
	using(var serviceScope = app.Services.CreateScope())
	{
		var serviceProvider = serviceScope.ServiceProvider;
		try
		{
			// Call all of my seed files to be seed
			CategorySeed.Seed(serviceProvider);
			ProductSeed.Seed(serviceProvider);
			TeamSeed.Seed(serviceProvider);
			//Seeds.Seed(serviceProvider);
		}
		catch(Exception ex)
		{
			var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "An error occurred while seeding into database.");
		}
	}
}
