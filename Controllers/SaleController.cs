using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class SaleController : Controller
    {
        private readonly IPartyService _partyService;
        private readonly IItemService _itemService;
        private readonly ISaleService _saleService;

        public SaleController(IItemService itemService, IPartyService partyService, ISaleService saleService)
        {
            _itemService = itemService;
            _partyService = partyService;
            _saleService = saleService;
        }
        //private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public async Task<IActionResult> Sale()
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            var saleData = await _saleService.GetSaleDataListAsync();           
            return View(saleData);
        }
        [HttpGet]
        public async Task<IActionResult> GetParties()
        {
            var response = await _partyService.GetAllPartiesAsync("Customer");
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var response = await _itemService.GetAllItemsAsync();           
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemPrice(int id)
        {
            var response = await _itemService.GetItemByIdAsync(id);
            var itemPrice = response.Data?.Price ?? 0;
            return Json(new { price = itemPrice });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSalesData([FromBody] SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
                return BadRequest("Invalid sale data.");

            var response = await _saleService.SaveSalesDataAsync(request);

            if (!response.Success)
            {
                // Map error messages to proper HTTP codes
                if (response.Message.StartsWith("Invalid argument"))
                    return BadRequest(new { success = false, message = response.Message });

                if (response.Message.StartsWith("Database error"))
                    return StatusCode(StatusCodes.Status503ServiceUnavailable,
                        new { success = false, message = "Database is temporarily unavailable. Please try again later." });

                if (response.Message.StartsWith("Operation error"))
                    return Conflict(new { success = false, message = response.Message });

                // Fallback for unexpected errors
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = response.Message });
            }
               
            return Json(new { success = response.Success, message = response.Message, id = response.Data });              
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalesData([FromBody] SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
                return BadRequest("Invalid sale data.");

            var response = await _saleService.UpdateSalesDataAsync(request);

            if (!response.Success)
            {
                // Map error messages to proper HTTP codes
                if (response.Message.StartsWith("Invalid argument"))
                    return BadRequest(new { success = false, message = response.Message });

                if (response.Message.StartsWith("Database error"))
                    return StatusCode(StatusCodes.Status503ServiceUnavailable,
                        new { success = false, message = "Database is temporarily unavailable. Please try again later." });

                if (response.Message.StartsWith("Operation error"))
                    return Conflict(new { success = false, message = response.Message });

                // Fallback for unexpected errors
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = response.Message });
            }

            return Json(new { success = response.Success, message = response.Message, id = response.Data });    
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSalesData(int id)
        {
            var response = await _saleService.DeleteSalesDataAsync(id);

            if (!response.Success)
            {
                if (response.Data == false && response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    // Case: Nothing deleted
                    return NotFound(new { success = false, message = response.Message });
                }

                // Case: Some unexpected error
                return StatusCode(500, new { success = false, message = response.Message });
            }

            return Json(new { success = true, message = response.Message });
        }
    }
}
