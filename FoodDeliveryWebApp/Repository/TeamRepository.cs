using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Team team)
        {
            _context.Add(team);
            return Save();

        }

        public bool Delete(Team team)
        {
            _context.Remove(team);
            return Save();

        }

        public async Task<IEnumerable<Team>> GetAll()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.TeamId == id);
        }

        public async Task<Team> GetById(int id)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.TeamId == id);
        }
        public async Task<IEnumerable<Team>> GetTeamByName(string name)
        {
            return await _context.Teams.Where(n => n.Name.Contains(name)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public bool Update(Team team)
        {
            _context.Update(team);
            return Save();
        }
    }
}
