using AllInOneProject.Data;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllInOneProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IItemService _itemService;
        public CartController(ApplicationDbContext context, IItemService itemService) 
        {
            _context = context;
            _itemService = itemService;
        }
        private int? UserId => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> Cart()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _itemService.GetUserCartItemsAsync(UserId??0);
            return View(response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _itemService.AddToCartAsync(itemId, UserId ?? 0);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _itemService.RemoveFromCartAsync(itemId, UserId ?? 0);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }   
}
