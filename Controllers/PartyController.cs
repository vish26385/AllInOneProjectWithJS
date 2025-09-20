using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Services;
using AllInOneProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PartyController : Controller
    {
        private readonly IPartyService _partyService;
        public PartyController(IPartyService partyService)
        {
            _partyService = partyService;
        }
        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public async Task<IActionResult> Party()
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            var response = await _partyService.GetAllPartiesAsync(null);
            var vm = new PartyMasterViewModel
            {
                partyMasters = response.Data?.Select(dto => new PartyMasterViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Type = dto.Type
                }).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SaveParty(PartyMasterViewModel model)
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterRequest for Save and Update
            var request = new PartyMasterRequest
            {
               Name = model.Name,
               Type = model.Type
            };

            var response = await _partyService.SavePartyAsync(request);

            TempData["PartyMessage"] = response.Message;
            return RedirectToAction("Party");
        }
        [HttpPost]
        public async Task<IActionResult> EditParty(int id)
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterDTO for GetPartyByIdAsync and GetAllPartiesAsync
            var partyDto = await _partyService.GetPartyByIdAsync(id);
            var partiesDto = await _partyService.GetAllPartiesAsync(null);

            var vm = new PartyMasterViewModel
            {
                Id = partyDto.Data.Id,
                Name = partyDto.Data.Name,
                Type = partyDto.Data.Type,
                partyMasters = partiesDto.Data?.Select(dto => new PartyMasterViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Type = dto.Type
                }).ToList()
            };

            return View("Party",vm);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateParty(PartyMasterViewModel model)
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            var request = new PartyMasterRequest
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type
            };

            var response = await _partyService.UpdatePartyAsync(request);

            TempData["PartyMessage"] = response.Message;
            return RedirectToAction("Party");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteParty(int Id)
        {
            if (string.IsNullOrEmpty(UserId))
                return RedirectToAction("Login", "Account");

            var response = await _partyService.DeletePartyAsync(Id);

            TempData["PartyMessage"] = response.Message;
            return RedirectToAction("Party");
        }
    }
}

