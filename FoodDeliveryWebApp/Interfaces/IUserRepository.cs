using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAll();
        //Task<AppUser> GetByIdAsync(int id);
        //Task<IEnumerable<AppUser>> GetUserByName(string UserName);
        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
        Task<AppUser> GetById(string id);
        Task SaveAsync();
    }
}
