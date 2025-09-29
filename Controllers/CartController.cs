using AllInOneProject.Models;
using AllInOneProject.Services;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AllInOneProject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CartController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CartController> _logger;
        public CartController(UserManager<ApplicationUser> userManager, 
                             IItemService itemService, 
                             IOrderService orderService, 
                             ICartService cartService,
                             IWebHostEnvironment env,
                             ILogger<CartController> logger) 
        {
            _userManager = userManager;
            _itemService = itemService;
            _orderService = orderService;
            _cartService = cartService;
            _env = env;
            _logger = logger;
        }
        private string? UserId => _userManager.GetUserId(User);

        public async Task<IActionResult> Cart()
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            var response = await _itemService.GetUserCartItemsAsync(UserId);
            return View(response.Data);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int itemId)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.AddToCartAsync(itemId, UserId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return Unauthorized(new { message = "User not logged in" });

            var response = await _itemService.RemoveFromCartAsync(itemId, UserId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            //if (string.IsNullOrEmpty(UserId))
            //    return RedirectToAction("Login", "Account");

            // Get cart items of user
            var cartItems = await _cartService.GetCartItemsAsync(UserId);

            if (cartItems.Data == null || !cartItems.Data.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }

            // Save order
            var response = await _orderService.PlaceOrderAsync(UserId, cartItems.Data);
            var orderId = 0;
            if (response.Data == null)
            {
                TempData["Error"] = "Something went wrong while placing the order.";
                return RedirectToAction("Index", "Cart");
            }
            else
             orderId = response.Data?.Id ?? 0;

            if (orderId > 0)
            {
                // Clear cart after successful order
                await _cartService.ClearCartAsync(UserId);

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction("OrderConfirmation", new { id = orderId });
            }

            TempData["Error"] = "Something went wrong while placing the order.";
            return RedirectToAction("Index", "Cart");
        }
        // Order confirmation page
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (response.Data == null)
            {
                TempData["Error"] = "Something went wrong while placing the order.";
                return RedirectToAction("Index", "Cart");
            }
            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            try
            {
                var response = await _orderService.GetOrderByIdAsync(id);
                if (response.Data == null) return NotFound();
                var order = response.Data;

                using (var ms = new MemoryStream())
                {
                    var writer = new PdfWriter(ms);
                    var pdf = new PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf, PageSize.A4);
                    document.SetMargins(20, 20, 20, 20);

                    //// ✅ Load Unicode font that supports ₹ symbol
                    //var fontPath = System.IO.Path.Combine(
                    //    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                    //    "arial.ttf"
                    //);
                    var fontPath = System.IO.Path.Combine(_env.WebRootPath, "fonts", "arial.ttf");

                    var regularFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                    //// Fonts: use built-in standard fonts (no external files required)
                    //var boldFontPath = System.IO.Path.Combine(
                    //    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                    //    "ARIALBD.TTF"   // Arial Bold (works with ₹ too if font supports it)
                    //);
                    var boldFontPath = System.IO.Path.Combine(_env.WebRootPath, "fonts", "arialbd.ttf");

                    var boldFont = PdfFontFactory.CreateFont(boldFontPath, PdfEncodings.IDENTITY_H);

                    // Header: Company title + invoice label
                    var header = new Paragraph("My Store")
                        .SetFont(boldFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(header);

                    var subtitle = new Paragraph("Invoice")
                        .SetFont(boldFont)
                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(10);
                    document.Add(subtitle);

                    // Company address / GST (example) - left aligned
                    var companyInfo = new Paragraph("My Store Pvt. Ltd.\n123, Your Street, Your City - 400001\nGSTIN: 12ABCDE3456F7Z8")
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(10);
                    document.Add(companyInfo);

                    // Order meta: two-column table (left: order id/date, right: total)
                    var infoTable = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                        .UseAllAvailableWidth()
                        .SetMarginBottom(10);

                    document.Add(new Paragraph($"Order ID: {order.Id}")
                                    .SetFont(regularFont)
                                    .SetFontSize(12));
                    document.Add(new Paragraph($"Order Date: {order.OrderDate:dd MMM yyyy, hh:mm tt}")
                        .SetFont(regularFont)
                        .SetFontSize(12));

                    document.Add(infoTable);

                    // Items table
                    var table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 1, 2 }))
                        .UseAllAvailableWidth();

                    // Header cells with bold font
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Item Name").SetFont(boldFont)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(220, 241, 230)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Price (₹)").SetFont(boldFont)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(220, 241, 230)).SetTextAlignment(TextAlignment.RIGHT));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFont(boldFont)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(220, 241, 230)).SetTextAlignment(TextAlignment.CENTER));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Subtotal (₹)").SetFont(boldFont)).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(220, 241, 230)).SetTextAlignment(TextAlignment.RIGHT));

                    // Rows
                    var culture = new CultureInfo("en-IN");
                    foreach (var item in order.OrderItems)
                    {
                        var name = item.Item?.Name ?? "—";
                        var priceStr = $"₹{item.Price.ToString("N2", culture)}";
                        var qtyStr = item.Quantity.ToString();
                        var subtotalStr = $"₹{(item.Price * item.Quantity).ToString("N2", culture)}";

                        table.AddCell(new Cell().Add(new Paragraph(name).SetFont(regularFont)));
                        table.AddCell(new Cell().Add(new Paragraph(priceStr).SetFont(regularFont)).SetTextAlignment(TextAlignment.RIGHT));
                        table.AddCell(new Cell().Add(new Paragraph(qtyStr).SetFont(regularFont)).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(subtotalStr).SetFont(regularFont)).SetTextAlignment(TextAlignment.RIGHT));
                    }

                    // Total row (span 3 columns)
                    var totalLabelCell = new Cell(1, 3).Add(new Paragraph("Total Amount").SetFont(boldFont)).SetTextAlignment(TextAlignment.RIGHT);
                    table.AddCell(totalLabelCell);
                    table.AddCell(new Cell().Add(new Paragraph($"₹{order.TotalAmount.ToString("N2", culture)}").SetFont(boldFont)).SetTextAlignment(TextAlignment.RIGHT));

                    document.Add(table);

                    // Footer / thanks
                    document.Add(new Paragraph("\nThank you for shopping with us!")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont)
                        .SetFontSize(10));

                    document.Close();

                    var fileName = $"Invoice_{order.Id}.pdf";
                    return File(ms.ToArray(), "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                // Or use ILogger
                _logger.LogError(ex.StackTrace, "Error generating invoice PDF for order {OrderId}", id);
                return StatusCode(500, "PDF generation failed.");
            }            
        }

        [HttpGet]
        [AllowAnonymous] // allow everyone to call this
        public async Task<IActionResult> GetCartCount()
        {
            // if user not logged in, just return 0
            if (UserId == null)
            {
                return Json(new { itemCount = 0 });
            }
            var response = await _cartService.GetCartItemsAsync(UserId);
            int itemCount = response.Data.Sum(c => c.Quantity);
            return Json(new { itemCount });
        }   
    }   
}
