using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AllInOneProject.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == request.UserName))
                return 0;

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
                Role = "User"
            };
            // Hash the password
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            _context.Users.Add(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<User?> ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user == null ? null : user;
        }

        public async Task<int> LoginAsync(RegisterRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Password is correct, proceed with login
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                        // add more claims if needed
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Issue the authentication cookie
                    await _httpContextAccessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties { IsPersistent = true });

                    _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                    _httpContextAccessor.HttpContext.Session.SetString("Role", user.Role);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }
    }
}
