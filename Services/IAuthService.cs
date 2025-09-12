using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterRequest request);
        Task<User?> ForgotPasswordAsync(string email);
        Task<int> LoginAsync(RegisterRequest request);
    }
}
