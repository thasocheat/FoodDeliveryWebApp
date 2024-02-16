using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Controllers.Backend
{

    public class SaleReportController : Controller
    {

        private readonly ApplicationDbContext _context;

        public SaleReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Reports()
        {
            var dailyReportData = GetDailyReportData();

            return View(dailyReportData);
        }

        public List<OrderViewModel> GetDailyReportData()
        {
            // Determine today's date
            DateTime today = DateTime.Today;

            // Set the start and end date to cover the entire day
            DateTime startDate = today.Date; // Start of the day
            DateTime endDate = today.Date.AddDays(1).AddTicks(-1); // End of the day

            // Retrieve daily report data from the database based on the date range
            var dailyReportData = _context.Orders
                .Include(o => o.Product)
                .Where(report => report.OrderAt >= startDate && report.OrderAt <= endDate)
                .ToList();

            // Create a list to store the view models
            var dailyReportViewModels = new List<OrderViewModel>();

            // Iterate through each order
            foreach (var order in dailyReportData)
            {
                // Calculate the total amount for the order
                double totalAmount = order.Quantity * order.Product.Price;

                // Create a new OrderViewModel instance and populate its properties
                var orderViewModel = new OrderViewModel
                {
                    OrderId = order.OrderId,
                    OrderNo = order.OrderNo,
                    OrderAt = order.OrderAt,
                    Status = order.Status,
                    TotalAmount = totalAmount,
                    // Add other properties as needed
                };

                // Add the OrderViewModel to the list
                dailyReportViewModels.Add(orderViewModel);
            }

            return dailyReportViewModels;
        }


        // Action method to return the partial view for the daily report
        [HttpPost]
        public IActionResult GetDailyReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Query the database for orders within the specified date range
                var dailyOrders = _context.Orders
                    .Where(o => o.OrderAt >= startDate && o.OrderAt <= endDate)
                    .ToList();

                // Pass the orders to the partial view for rendering
                return View("Reports", dailyOrders);
            }
            catch (Exception ex)
            {
                // Handle any errors and return an error message
                return StatusCode(500, $"Error retrieving daily report: {ex.Message}");
            }
        }


        public IActionResult WeeklyReports()
        {
            // Get today's date
            DateTime today = DateTime.Today;

            // Calculate the start and end dates of the current week
            DateTime startDateOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6);

            // Query the database for weekly report data
            var weeklyReportData = GetWeeklyReportData(startDateOfWeek, endDateOfWeek);

            return View(weeklyReportData);
        }

        private List<OrderViewModel> GetWeeklyReportData(DateTime startDate, DateTime endDate)
        {
            // Retrieve weekly report data from the database based on the date range
            var weeklyReportData = _context.Orders
                .Include(o => o.Product)
                .Where(report => report.OrderAt >= startDate && report.OrderAt <= endDate)
                .ToList();

            // Create a list to store the view models
            var weeklyReportViewModels = new List<OrderViewModel>();

            // Iterate through each order
            foreach (var order in weeklyReportData)
            {
                // Calculate the total amount for the order
                double totalAmount = order.Quantity * order.Product.Price;

                // Create a new OrderViewModel instance and populate its properties
                var orderViewModel = new OrderViewModel
                {
                    OrderId = order.OrderId,
                    OrderNo = order.OrderNo,
                    OrderAt = order.OrderAt,
                    Status = order.Status,
                    TotalAmount = totalAmount,
                    // Add other properties as needed
                };

                // Add the OrderViewModel to the list
                weeklyReportViewModels.Add(orderViewModel);
            }

            return weeklyReportViewModels;
        }

        private DateTime GetStartDateOfWeek(int weekNumber, int year, int month)
        {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime firstDayOfFirstWeek = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            int daysToAdd = (weekNumber - 1) * 7;
            return firstDayOfFirstWeek.AddDays(daysToAdd);
        }

        // Action method to return the partial view for the weekly report
        [HttpPost]
        public IActionResult GetWeeklyReport(int weekNumber, int year, int month)
        {
            try
            {
                // Calculate the start and end dates of the selected week within the specified month and year
                DateTime startDate = GetStartDateOfWeek(weekNumber, year, month);
                DateTime endDate = startDate.AddDays(6);

                // Query the database for weekly report data
                var weeklyReportData = GetWeeklyReportData(startDate, endDate);

                // Pass the orders to the partial view for rendering
                return PartialView("WeeklyReports", weeklyReportData);
            }
            catch (Exception ex)
            {
                // Handle any errors and return an error message
                return StatusCode(500, $"Error retrieving weekly report: {ex.Message}");
            }
        }


        public IActionResult MonthlyReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Query the database for orders within the specified date range
                var monthlyOrders = _context.Orders
                    .Where(o => o.OrderAt >= startDate && o.OrderAt <= endDate)
                    .ToList();

                // Pass the orders to the view for rendering
                return View("Reports", monthlyOrders);
            }
            catch (Exception ex)
            {
                // Handle any errors and return an error message
                return StatusCode(500, $"Error retrieving monthly report: {ex.Message}");
            }
        }


        public IActionResult YearlyReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Query the database for orders within the specified date range
                var yearlyOrders = _context.Orders
                    .Where(o => o.OrderAt >= startDate && o.OrderAt <= endDate)
                    .ToList();

                // Pass the orders to the view for rendering
                return View("Reports", yearlyOrders);
            }
            catch (Exception ex)
            {
                // Handle any errors and return an error message
                return StatusCode(500, $"Error retrieving yearly report: {ex.Message}");
            }
        }





    }
}
