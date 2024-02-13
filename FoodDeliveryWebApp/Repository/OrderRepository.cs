using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using FoodDeliveryWebApp.ViewModels;

namespace FoodDeliveryWebApp.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateOrders(List<OrderViewModel> orders)
        {
            // Assuming Order entity matches OrderViewModel properties
            var orderEntities = orders.Select(orderViewModel => new Order
            {
                ProductId = orderViewModel.ProductId,
                Quantity = orderViewModel.Quantity,
                UserId = orderViewModel.UserId,                
            }).ToList();

            // Add orders to the database context and save changes
            _context.Orders.AddRange(orderEntities);
            await _context.SaveChangesAsync();
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }
    }
}
