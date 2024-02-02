namespace FoodDeliveryWebApp.ViewModels
{
    public class CartViewModel
    {
        public int CartItemCount { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Add KhmerPrice and KhmerFormattedTotal properties
        public double KhmerPrice { get; set; }
        public double USDPrice { get; set; }
        public double Price { get; set; }
        public string KhmerFormattedTotal => $"{Quantity * KhmerPrice:C}";
        public string FormattedPrice { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string FormattedTotal { get; set; }
    }
}
