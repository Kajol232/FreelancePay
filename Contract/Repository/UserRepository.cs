using FreelancePay.Contract.Models;
using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelancePay.Contract.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser>? GetUserByUsernameAsync(string username)
        {
            return await _context.AppUsers.SingleOrDefaultAsync(u => u.Username == username);
            
        }
        public async Task<AppUser>? GetUserByEmailAsync(string email)
        {
            return await _context.AppUsers.SingleOrDefaultAsync(u => u.Email == email);

        }
        public async Task<AppUser> AddUserAsync(AppUser user)
        {
            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.Username == username);
        }

        public async Task<List<AppUser>> GetUsersInRoleAsync(Role role)
        {
            return await _context.AppUsers.Where(u => u.Role == role).ToListAsync();
        }
        public async Task<IEnumerable<ClientViewModel>> GetClientsAsync()
        {
            return await _context.AppUsers
                .Where(u => u.Role == Role.Client)
                .Select(u => new ClientViewModel
                {
                    ClientId = u.Id,
                    ClientName = u.Username,
                }).ToListAsync();
        }

      
    }
}
