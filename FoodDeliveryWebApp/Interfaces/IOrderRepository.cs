using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;

namespace FoodDeliveryWebApp.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateOrders(List<OrderViewModel> orders);
        void AddOrder(Order order);
        Order GetOrderById(int orderId);
    }
}
