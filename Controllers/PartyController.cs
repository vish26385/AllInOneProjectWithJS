using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace AllInOneProject.Controllers
{
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
            var im = new PartyMasterRequest
            {
                partyMasters = response.Data
            };
            return View(im);
        }
        [HttpPost]
        public async Task<IActionResult> SaveParty(PartyMasterRequest request)
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");

            var response = await _partyService.SavePartyAsync(request);

            ViewBag.Message = response.Message;
            return RedirectToAction("Party");
        }
        [HttpPost]
        public async Task<IActionResult> EditParty(int Id)
        {
            var model = await _partyService.GetEditPartyModelAsync(Id);
            return View("Party", model); // return same view
        }
        [HttpPost]
        public async Task<IActionResult> UpdateParty(PartyMasterRequest request)
        {
            if(UserId == null)
                return RedirectToAction("Login", "Account");

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

