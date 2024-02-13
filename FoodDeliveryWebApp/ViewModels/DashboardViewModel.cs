using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.ViewModels
{
    public class DashboardViewModel
    {
        public int UserCount { get; set; }
        public int AdminCount { get; set; }
        public int StaffCount { get; set; }
        public int CartCount { get; set; }
        public int CategoryCount { get; set; }
        public int OrderCount { get; set; }
        public int PaymentCount { get; set; }
        public int TeamCount { get; set; }
        public double TotalIncome { get; set; }
        public double TotalIncomeKH { get; set; }

        public List<Order> LatestOrders { get; set; }
    }
}
