using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public DashboardController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var adminRole = "Admin";
            var staffRole = "Staff";

            
            // Get counts of users, admins, staff, carts, categories, orders, payments, teams
            var userCount = _context.Users.Count();
            var adminCount = _userManager.GetUsersInRoleAsync(adminRole).Result.Count();
            var staffCount = _userManager.GetUsersInRoleAsync(staffRole).Result.Count();
            var cartCount = _context.Carts.Count();
            var categoryCount = _context.Categories.Count();
            var orderCount = _context.Orders.Count();
            var paymentCount = _context.Payments.Count();
            var teamCount = _context.Teams.Count();

            // Calculate total income from user orders
            var totalIncome = _context.Orders.Sum(o => o.Product.Price * o.Quantity);
            var TotalIncomeKH = _context.Orders.Sum(o => o.Product.Price * o.Quantity * 4100);

            // Get latest orders
            var latestOrders = _context.Orders
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderAt)
                .Take(10).ToList();

            // Pass the counts and totals to the view
            var viewModel = new DashboardViewModel
            {
                UserCount = userCount,
                AdminCount = adminCount,
                StaffCount = staffCount,
                CartCount = cartCount,
                CategoryCount = categoryCount,
                OrderCount = orderCount,
                PaymentCount = paymentCount,
                TeamCount = teamCount,
                TotalIncome = totalIncome,
                LatestOrders = latestOrders
            };

            return View(viewModel);
        }
    }
}
