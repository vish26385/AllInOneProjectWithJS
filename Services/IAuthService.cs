using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<UserDto>> RegisterAsync(RegisterRequest request);
        Task<bool> ForgotPasswordAsync(string email);
        Task<ServiceResponse<LoginResultDto>> LoginAsync(LoginRequest request);
    }
}
