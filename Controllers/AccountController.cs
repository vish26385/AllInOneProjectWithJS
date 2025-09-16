using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AllInOneProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                return BadRequest(new { success = false, message = string.Join("\n", errors) });
            }

            var response = await _authService.RegisterAsync(request);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _authService.ForgotPasswordAsync(email);
            if (user)
            {
                return Json($"Reset link sent to {email}");
            }
            else
                return Json("Email not found");
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)  
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                return BadRequest(new { success = false, message = string.Join("\n", errors) });
            }

            var response = await _authService.LoginAsync(request);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
