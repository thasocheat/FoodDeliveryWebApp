using FoodDeliveryWebApp.Data;
using FoodDeliveryWebApp.Interfaces;
using FoodDeliveryWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(AppUser user)
        {
            _context.Add(user);
            return Save();
        }

        public bool Delete(AppUser user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<AppUser>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        //public async Task<AppUser> GetByIdAsync(int id)
        //{
        //    return await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
        //}

        //public async Task<IEnumerable<AppUser>> GetCateByName(string userName)
        //{
        //    return await _context.AppUsers.Where(u => u.UserName.Contains(userName)).ToListAsync();
        //}

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}

