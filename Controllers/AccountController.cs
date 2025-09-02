using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthRepository _authRepository;
        public AccountController(ApplicationDbContext context, IAuthRepository authRepository)
        {
            _context = context;
            _authRepository = authRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authRepository.RegisterAsync(request);

            if (response == 0)
                return Json("UserName exists");
            else
                return Json("Registered");
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _authRepository.ForgotPasswordAsync(email);
            if (user != null)
            {
                return Json($"Reset link sent to {email}");
            }
            else
                return Json("Email not found");
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] RegisterRequest request)  
        {
            var response = await _authRepository.LoginAsync(request);

            if (response == 0)
                return Json("Invalid Credentials!");
            else
                return Json("Success");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
