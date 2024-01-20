using FoodDeliveryWebApp.Models;

namespace FoodDeliveryWebApp.Interfaces
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAll();
        Task<Team> GetByIdAsync(int id);
        Task<IEnumerable<Team>> GetTeamByName(string  name);
        bool Add(Team team);
        bool Update(Team team);
        bool Delete(Team team);
        bool Save();
        Task<Team> GetById(int id);
        Task SaveAsync();
    }
}
