using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllInOneProject.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly IPartyService _partyService;
        private readonly IItemService _itemService;
        private readonly ISaleService _saleService;

        public SaleController(ApplicationDbContext context, IConfiguration configuration, IItemService itemService, IPartyService partyService, ISaleService saleService)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _itemService = itemService;
            _partyService = partyService;
            _saleService = saleService;
        }
        private int? UserId => HttpContext.Session.GetInt32("UserId");
        public async Task<IActionResult> Sale()
        {
            if (UserId == null)
                return RedirectToAction("Login", "Account");
            ////****Type Of IOrderedQueryable<SalesMaster>****////
            //var query = from s in _context.SalesMas
            //                .Include(s => s.PartyMaster)
            //                .Include(s => s.salesDetails)
            //                    .ThenInclude(d => d.ItemMaster)
            //            orderby s.Id descending
            //            select s;

            ////****Type Of IEnumerable<SalesMaster>****////
            //var query = (from s in _context.SalesMas
            //             join p in _context.PartyMasters
            //                 on s.PartyId equals p.Id
            //             join d in _context.SalesDetails
            //                 on s.Id equals d.SaleMasterId
            //             join i in _context.ItemMasters
            //                 on d.ItemId equals i.Id
            //             orderby s.Id descending
            //             select new
            //             {
            //                 SalesMaster = s,
            //                 PartyMaster = p,
            //                 SalesDetail = d,
            //                 ItemMaster = i
            //             })
            //            .AsEnumerable() // switch to LINQ-to-Objects for grouping
            //            .GroupBy(x => x.SalesMaster.Id)
            //            .Select(g =>
            //            {
            //                var sm = g.First().SalesMaster;
            //                sm.PartyMaster = g.First().PartyMaster;
            //                sm.salesDetails = g.Select(d =>
            //                {
            //                    d.SalesDetail.ItemMaster = d.ItemMaster;
            //                    return d.SalesDetail;
            //                }).ToList();
            //                return sm;
            //            });

            ////****Type Of IQueryable<a> means IQueryable<SalesMaster SalesMaster, PartyMaster PartyMaster, SalesDetail SalesDetail, ItemMaster ItemMaster>****////
            //var query = from s in _context.SalesMas
            //            join p in _context.PartyMasters
            //                on s.PartyId equals p.Id
            //            join d in _context.SalesDetails
            //                on s.Id equals d.SaleMasterId
            //            join i in _context.ItemMasters
            //                on d.ItemId equals i.Id
            //            orderby s.Id descending
            //            select new
            //            {
            //                SalesMaster = s,
            //                PartyMaster = p,
            //                SalesDetail = d,
            //                ItemMaster = i
            //            };

            //var saleData = query.ToList() //For above First Two

            //var saleData = query //For above Third
            //            .AsEnumerable() // switch to LINQ-to-Objects for grouping
            //            .GroupBy(x => x.SalesMaster.Id)
            //            .Select(g =>
            //            {
            //                var sm = g.First().SalesMaster;
            //                sm.PartyMaster = g.First().PartyMaster;
            //                sm.salesDetails = g.Select(d =>
            //                {
            //                    d.SalesDetail.ItemMaster = d.ItemMaster;
            //                    return d.SalesDetail;
            //                }).ToList();
            //                return sm;
            //            })
            //            .ToList()

            var saleData = await _context.SalesMas
                            .Include(sm => sm.PartyMaster)
                            .Include(sm => sm.salesDetails)
                            .ThenInclude(sd => sd.ItemMaster)
                            .Select(sm => new
                            {
                                Id = sm.Id,
                                SaleDate = sm.SalesDate.ToString("yyyy-MM-dd"),
                                DueDays = sm.DueDays,
                                DueDate = sm.DueDate.ToString("yyyy-MM-dd"),
                                PartyId = sm.PartyId,
                                PartyName = sm.PartyMaster.Name,
                                Qty = sm.salesDetails.Sum(sd => sd.Qty),
                                Amount = Convert.ToDecimal(sm.salesDetails.Sum(sd => sd.Qty * sd.ItemMaster.Price)),
                                salesDetails = sm.salesDetails.Select(sd => new
                                {
                                    sd.Id,
                                    sd.itemId,
                                    sd.SalesMasterId,
                                    sd.Qty
                                }).ToList()
                            }).ToListAsync();

            return View(saleData);
        }
        [HttpGet]
        public async Task<IActionResult> GetParties()
        {
            var response = await _partyService.GetAllPartiesAsync();
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
                return StatusCode(500, new { success = false, message = response.Message });

            return Json(new { success = response.Success, message = response.Message, id = response.Data });              
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalesData([FromBody] SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
                return BadRequest("Invalid sale data.");

            var response = await _saleService.UpdateSalesDataAsync(request);

            if (!response.Success)
                return StatusCode(500, new { success = false, message = response.Message });

            return Json(new { success = response.Success, message = response.Message, id = response.Data });    
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSalesData(int id)
        {
            var response = await _saleService.DeleteSalesDataAsync(id);

            if (!response.Success)
                return StatusCode(500, new { success = false, message = response.Message });

            return Json(new { success = true, message = "Data deleted successfully." });
        }
    }
}
