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


        //[HttpGet]
        //public async Task<IActionResult> AddToCart()
        //{
        //    var user 
        //}


        [HttpPost]
        public async Task<IActionResult> AddToCart(int templateId)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return RedirectToAction("Login", "Account");
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

            return RedirectToAction("Index", "Cart");
        }
    }
}
