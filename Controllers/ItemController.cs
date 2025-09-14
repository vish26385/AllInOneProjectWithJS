using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Services;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
        private int? UserId => HttpContext.Session.GetInt32("UserId");
        public async Task<IActionResult> Item()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _itemService.GetAllItemsAsync();
            return View(response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> InsertItem([FromBody] ItemRequest request)
        {
            if (UserId == null)
                return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.InsertItemAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] ItemRequest request)
        {
            if (UserId == null)
                return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.UpdateItemAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItem([FromBody] int id)
        {
            if (UserId == null)
                return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.DeleteItemAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
