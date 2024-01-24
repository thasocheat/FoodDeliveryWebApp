using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetCateByName(string Categoryname);
        //Task<IEnumerable<Category>> GetCateByIsActive(bool isActive);
        bool Add(Category category);
        bool Update(Category category);
        bool Delete(Category category);
        bool Save();
        Task<Category> GetById(int id);
        Task SaveAsync();
        //void SaveChanges();
    }
}
