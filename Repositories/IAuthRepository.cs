using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> ExistsAsync(string userName);
        Task<User?> GetByUserNameAsync(string userName);
        Task<bool> RegisterAsync(User user);
        Task<User?> GetByEmailAsync(string email);
    }
}
