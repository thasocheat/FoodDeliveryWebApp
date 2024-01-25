using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetById(int id);
        Task<IEnumerable<Product>> GetTeamByName(string name);
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(Product product);
        bool Save();
    }
}
