namespace FoodDeliveryWebApp.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; }
        public string ExpiryDate { get; set; }
        public int CvvNo { get; set; }
        public string UserId { get; set; }
        public string Address { get; set; }
        public string PaymentMode { get; set; }
        public DateTime PayAt { get; set; }

        // Additional properties for presentation, if needed
        public double TotalAmount { get; set; }
    }
}
