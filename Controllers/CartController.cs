using AllInOneProject.Data;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CartController : Controller
    {
        private readonly IItemService _itemService;
        public CartController(IItemService itemService) 
        {
            _itemService = itemService;
        }
        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Cart()
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            var response = await _itemService.GetUserCartItemsAsync(Convert.ToInt32(UserId));
            return View(response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.AddToCartAsync(itemId, Convert.ToInt32(UserId));

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.RemoveFromCartAsync(itemId, Convert.ToInt32(UserId));

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }   
}
