namespace FoodDeliveryWebApp.ViewModels
{
    public class OrderSummaryViewModel
    {
        public OrderViewModel Order { get; set; }
        public PaymentViewModel Payment { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
