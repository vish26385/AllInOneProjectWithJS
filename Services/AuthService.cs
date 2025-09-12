using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AllInOneProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IAuthRepository authRepository, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> RegisterAsync(RegisterRequest request)
        {
            if (await _authRepository.ExistsAsync(request.UserName))
                return 0;

            var user = new User
            {
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                Role = "User"
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            var saved = await _authRepository.RegisterAsync(user);
            return saved ? 1 : -1;
        }
        public async Task<User?> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            return user == null ? null : user;
        }

        public async Task<int> LoginAsync(RegisterRequest request)
        {
            var user = await _authRepository.GetByUserNameAsync(request.UserName);
            if (user == null)
                return 0;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result != PasswordVerificationResult.Success)
                return 0;

            // ✅ password is correct, issue claims & cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });

            _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
            _httpContextAccessor.HttpContext.Session.SetString("Role", user.Role);

            return 1;
        }
    }
}
