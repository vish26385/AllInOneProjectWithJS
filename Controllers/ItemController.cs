using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }
        //private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);   
        public async Task<IActionResult> Item()
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            var response = await _itemService.GetAllItemsAsync();
            return View(response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> InsertItem([FromBody] ItemRequest request)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return Unauthorized(new { message = "User not logged in" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                return BadRequest(new { success = false, message = string.Join("\n", errors) });
            }

            var response = await _itemService.InsertItemAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] ItemRequest request)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return Unauthorized(new { message = "User not logged in" });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

                return BadRequest(new { success = false, message = string.Join("\n", errors) });
            }

            var response = await _itemService.UpdateItemAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItem([FromBody] int id)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.DeleteItemAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
