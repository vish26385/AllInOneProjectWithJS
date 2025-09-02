using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IAuthRepository
    {
        Task<int> RegisterAsync(RegisterRequest request);
        Task<User?> ForgotPasswordAsync(string email);
        Task<int> LoginAsync(RegisterRequest request);
    }
}
