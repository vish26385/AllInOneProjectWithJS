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

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PartyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly IPartyService _partyService;
        public PartyController(ApplicationDbContext context, IConfiguration configuration, IPartyService partyService)
        {
            _context = context;
            _partyService = partyService;
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }
        private int? UserId => HttpContext.Session.GetInt32("UserId");
        public async Task<IActionResult> Party()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _partyService.GetAllPartiesAsync();
            var vm = new PartyMasterViewModel
            {
                partyMasters = response.Data?.Select(dto => new PartyMasterViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveParty(PartyMasterViewModel model)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterRequest for Save and Update
            var request = new PartyMasterRequest
            {
               Name = model.Name               
            };

            var response = await _partyService.SavePartyAsync(request);

            ViewBag.Message = response.Message;
            return RedirectToAction("Party");
        }
        [HttpPost]
        public async Task<IActionResult> EditParty(int id)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterDTO for GetPartyByIdAsync and GetAllPartiesAsync
            var partyDto = await _partyService.GetPartyByIdAsync(id);
            var partiesDto = await _partyService.GetAllPartiesAsync();

            var vm = new PartyMasterViewModel
            {
                Id = partyDto.Data.Id,
                Name = partyDto.Data.Name,
                partyMasters = partiesDto.Data?.Select(dto => new PartyMasterViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name
                }).ToList()
            };

            return View("Party",vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParty(PartyMasterViewModel model)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO
            var request = new PartyMasterRequest
            {
                Id = model.Id,
                Name = model.Name                
            };

            var response = await _partyService.UpdatePartyAsync(request);

            ViewBag.Message = response.Message;
            return RedirectToAction("Party");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteParty(int Id)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _partyService.DeletePartyAsync(Id);

            ViewBag.Message = response.Message;
            return RedirectToAction("Party");
        }
    }
}

