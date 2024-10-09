using FreelancePay.Entities;
using FreelancePay.Contract.Models;
namespace FreelancePay.Contract.Repository
{
    public interface IUserRepository
    {
        Task<AppUser>? GetUserByUsernameAsync(string username);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser> AddUserAsync(AppUser user);
        Task<bool> UserExistsAsync(string username);
        Task<List<AppUser>> GetUsersInRoleAsync(Role role);
        Task<IEnumerable<ClientViewModel>> GetClientsAsync();
        
    }
}
