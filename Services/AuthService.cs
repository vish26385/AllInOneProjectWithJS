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

        public async Task<ServiceResponse<UserDto>> RegisterAsync(RegisterRequest request)
        {
            if (await _authRepository.ExistsAsync(request.UserName))
            {
                return new ServiceResponse<UserDto>
                {
                    Success = false,
                    Message = "Username already exists."
                };
            }

            try
            {
                var user = new User
                {
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    Role = "User"
                };

                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

                var result = await _authRepository.RegisterAsync(user);
                var userDto = new UserDto { Id = result.Id, UserName = result.UserName, Password = result.Password,
                                            PasswordHash = result.PasswordHash, Email = result.Email, Role = result.Role};

                return new ServiceResponse<UserDto>
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserDto>
                {
                    Success = false,
                    Message = $"Error registering user: {ex.Message}"
                };
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            return user == null ? false : true;
        }

        public async Task<ServiceResponse<LoginResultDto>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _authRepository.GetByUserNameAsync(request.UserName);
                if (user == null)
                {
                    return new ServiceResponse<LoginResultDto>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    return new ServiceResponse<LoginResultDto>
                    {
                        Success = false,
                        Message = "Invalid Credentials!"
                    };
                }

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

                //_httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.Id);
                //_httpContextAccessor.HttpContext.Session.SetString("Role", user.Role);

                var loginDto = new LoginResultDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Password = user.Password
                };

                return new ServiceResponse<LoginResultDto>
                {
                    Success = true,
                    Message = "Success",
                    Data = loginDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResultDto>
                {
                    Success = false,
                    Message = $"Error logging in: {ex.Message}"
                };
            }
        }
    }
}
