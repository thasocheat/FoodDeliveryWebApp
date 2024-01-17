using FoodDeliveryWebApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services for connecting to SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Run all seeder file for example user use
if(args.Length == 1 && args[0].ToLower() == "seeddata")
{
	//Seed.SeedData(app);
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
		}
		catch(Exception ex)
		{
			var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "An error occurred while seeding into database.");
		}
	}
}
