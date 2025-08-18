using MallarEmelieMVC.Areas.Identity.Models;
using MallarEmelieMVC.Data;
using MallarEmelieMVC.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MallarEmelieMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<IdentityUserTable> _userManager;
        private readonly AppDbContext _context;
        public CartController(UserManager<IdentityUserTable> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "Du måste vara inloggad för att se din kundvagn";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var userId = user.Id;

            var cartList = await _context.CartItems.Include(c => c.Template).Where(u => u.UserId == userId).ToListAsync();

            if(!cartList.Any())
            {
                TempData["Info"] = "Din kundvagn är tom.";
                return View(new List<CartItem>()); 
            }

            return View(cartList);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int templateId)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                TempData["Error"] = "Du måste vara inloggad för att lägga till i kundvagnen";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userId = user.Id;

            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.TemplateId == templateId);

            if(existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    UserId = userId,
                    TemplateId = templateId,
                    Quantity = 1
                };

                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Produkten lades till i kundvagnen!";

            return RedirectToAction("ViewTemplate", "Template");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemById)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Du måste logga in för att ta bort från kundvagnen";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(i => i.CartItemId == cartItemById && i.UserId == user.Id);
            if(cartItem != null)
            {
                _context.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkten har tagits bort från kundvagnen";
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int cartItemById)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                TempData["Error"] = "Du måste logga in för att ändra i kundvagnen";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cartItemToIncrease = await _context.CartItems.FirstOrDefaultAsync(c => c.CartItemId == cartItemById && user.Id == c.UserId);
            if(cartItemToIncrease == null)
            {
                TempData["Error"] = "Produkten kunde inte hittas";
                return RedirectToAction("ViewCart");
            }

            cartItemToIncrease.Quantity++;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kundvagnen har uppdaterats";
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int cartItemById)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["Error"] = "Du måste logga in för att ändra i kundvagnen";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cartItemToDecrease = await _context.CartItems.FirstOrDefaultAsync(c => c.CartItemId == cartItemById && c.UserId == user.Id);
            if(cartItemToDecrease == null)
            {
                TempData["Error"] = "Produkten kunde inte hittas";
                return RedirectToAction("ViewCart");
            }

            if(cartItemToDecrease.Quantity <= 1)
            {
                TempData["Error"] = "Produktens antal går inte att göra mindre än 1";
                return RedirectToAction("ViewCart");
            }

            cartItemToDecrease.Quantity--;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kundvagnen har uppdaterats";
            return RedirectToAction("ViewCart");
        }
    }
}
