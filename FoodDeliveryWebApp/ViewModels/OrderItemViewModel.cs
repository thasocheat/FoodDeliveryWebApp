﻿namespace FoodDeliveryWebApp.ViewModels
{
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
    }
}