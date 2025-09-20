using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using AllInOneProject.Services;
using AllInOneProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly IPartyService _partyService;
        private readonly IItemService _itemService;
        private readonly IPurchaseService _purchaseService;
        public PurchaseController(ApplicationDbContext context, IConfiguration configuration, IPurchaseService purchaseService,
            IItemService itemService, IPartyService partyService)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _itemService = itemService;
            _partyService = partyService;
            _purchaseService = purchaseService;
        }
        //private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public async Task<IActionResult> Purchase()
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            var responseItems = await _itemService.GetAllItemsAsync();
            var responseParties = await _partyService.GetAllPartiesAsync("Supplier");
            var purchaseData = await _purchaseService.GetPurchaseDataListAsync();

            var pm = new PurchaseViewModel
            {
                itemMasters = ViewBag.Items = responseItems.Data,
                partyMasters = responseParties.Data,
                purchaseLists = purchaseData.Data
            };
            pm.purchaseMaster = new PurchaseDTO();
            pm.purchaseMaster.PurchaseDate = DateTime.Today;
            if (pm.purchaseMaster.purchaseDetails == null || pm.purchaseMaster.purchaseDetails.Count == 0)
            {
                pm.purchaseMaster.purchaseDetails.Add(new PurchaseDetailDTO());
            }
            return View(pm);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(PurchaseViewModel model)
        {
            var responseItems = await _itemService.GetAllItemsAsync();
            var responseParties = await _partyService.GetAllPartiesAsync("Supplier");
            var purchaseData = await _purchaseService.GetPurchaseDataListAsync();

            model.itemMasters = ViewBag.Items = responseItems.Data;
            model.partyMasters = responseParties.Data;
            model.purchaseLists = purchaseData.Data;

            foreach (var detail in model.purchaseMaster.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    var selectedItem = model.itemMasters.FirstOrDefault(x => x.Id == detail.ItemId);
                    if (selectedItem != null)
                    {
                        detail.Rate = selectedItem.Price;
                        detail.Amount = detail.Qty * selectedItem.Price;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePurchase(PurchaseViewModel model)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterRequest for Save and Update
            var request = new PurchaseMasterRequest
            {
                PurchaseDate = model.purchaseMaster.PurchaseDate,
                PartyMasterId = model.purchaseMaster.PartyMasterId,
                PurchaseDetailRequests = model.purchaseMaster.purchaseDetails.Select(d => new PurchaseDetailRequest
                {
                    PurchaseMasterId = d.PurchaseMasterId,
                    ItemId = d.ItemId,
                    Qty = d.Qty
                }).ToList()
            };

            var response = await _purchaseService.SavePurchaseDataAsync(request);

            TempData["PurchaseMessage"] = response.Message;
            return RedirectToAction("Purchase");            
        }

        [HttpPost]
        public async Task<IActionResult> EditPurchase(int id)
        {
            var master = await _purchaseService.GetPurchaseMasterDataByIdAsync(id);
            var responseItems = await _itemService.GetAllItemsAsync();
            var responseParties = await _partyService.GetAllPartiesAsync("Supplier");
            var purchaseData = await _purchaseService.GetPurchaseDataListAsync();

            var model = new PurchaseViewModel
            {
                purchaseMaster = master.Data,
                itemMasters = ViewBag.Items = responseItems.Data,
                partyMasters = responseParties.Data,
                purchaseLists = purchaseData.Data
            };
            return View("Purchase", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePurchase(PurchaseViewModel model)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            // Map ViewModel → DTO //User PartyMasterRequest for Save and Update
            var request = new PurchaseMasterRequest
            {
                Id = model.purchaseMaster.Id,
                PurchaseDate = model.purchaseMaster.PurchaseDate,
                PartyMasterId = model.purchaseMaster.PartyMasterId,
                PurchaseDetailRequests = model.purchaseMaster.purchaseDetails.Select(d => new PurchaseDetailRequest
                {
                    PurchaseMasterId = d.PurchaseMasterId,
                    ItemId = d.ItemId,
                    Qty = d.Qty
                }).ToList()
            };
            var response = await _purchaseService.UpdatePurchaseDataAsync(request);

            TempData["PurchaseMessage"] = response.Message;
            return RedirectToAction("Purchase");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            var response = await _purchaseService.DeletePurchaseDataAsync(id);

            TempData["PurchaseMessage"] = response.Message;
            return RedirectToAction("Purchase");
        }
    }
}
