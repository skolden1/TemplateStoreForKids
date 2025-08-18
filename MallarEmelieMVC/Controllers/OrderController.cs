using MallarEmelieMVC.Areas.Identity.Models;
using MallarEmelieMVC.Data;
using MallarEmelieMVC.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MallarEmelieMVC.Controllers
{
    //Bara inloggade kommer åt denna ctrl
    [Authorize] 
    public class OrderController : Controller
    {
        private readonly UserManager<IdentityUserTable> _userManager;
        private readonly AppDbContext _context;
        public OrderController(UserManager<IdentityUserTable> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
       

        [HttpGet]
        public async Task<IActionResult> ViewOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Du måste logga in för att se dina ordrar";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var allOrders = await _context.Orders.Where(o => o.UserId == user.Id)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Template)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            if (!allOrders.Any())
            {
                TempData["Error"] = "Du har inga tidigare ordrar";
                return RedirectToAction("ViewTemplate", "Template");
            }

            return View(allOrders);

        }

        [HttpGet]
        public async Task<IActionResult> Receipt()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Du måste logga in för att se kvittot";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var latestOrder = await _context.Orders.Where(u => u.UserId == user.Id)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Template)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if(latestOrder == null)
            {
                TempData["Error"] = "Kunde inte hitta din order";
                return RedirectToAction("ViewCart", "Cart");
            }

            return View(latestOrder);
        }

        //för att visa checkout formuläret
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(string MobileNr, string StreetAddress, string PostalCode, string City)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Du måste logga in för att slutföra beställningen";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Template)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Din kundvagn är tom";
                return RedirectToAction("ViewCart", "Cart");
            }

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                PaymentComment = "Swisha till 0735266141, ange orderID som kommentar",
                Status = "Pending",
                MobileNr = MobileNr,
                StreetAddress = StreetAddress,
                PostalCode = PostalCode,
                City = City,
                Items = cartItems.Select(c => new OrderItem
                {
                    TemplateId = c.TemplateId,
                    Quantity = c.Quantity,
                    Price = c.Template.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Receipt", new { orderId = order.OrderId });
        }
    }
}
