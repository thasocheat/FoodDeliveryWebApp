namespace FoodDeliveryWebApp.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int? PaymentId { get; set; }
        public string? UserId { get; set; }
        public string ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; }
        public DateTime OrderAt { get; set; }

        // Additional properties for presentation, if needed
        public string Name { get; set; }
        public double Price { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalAmount { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
    }

}
