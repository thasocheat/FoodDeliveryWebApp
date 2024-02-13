namespace FoodDeliveryWebApp.ViewModels
{
    public class OrderAndPaymentViewModel
    {
        public OrderViewModel Order { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public PaymentViewModel Payment { get; set; }
    }
}
