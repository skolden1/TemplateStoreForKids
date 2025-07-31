using MallarEmelieMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MallarEmelieMVC.Controllers
{
    public class TemplateController : Controller
    {
        private readonly AppDbContext _context;
        public TemplateController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> ViewTemplate(string searchWord)
        {
            var allTemplates = await _context.Templates.Include(c => c.Category).ToListAsync();

            if (!string.IsNullOrEmpty(searchWord))
            {
                allTemplates = await _context.Templates.Where(t => t.Title.ToLower().Contains(searchWord.ToLower()) || t.Category.CategoryName.ToLower().Contains(searchWord.ToLower())).ToListAsync();
            }

            return View(allTemplates);     
                  
        }

        [HttpGet]
        public async Task<IActionResult> ViewTemplateById(int id)
        {
            var tempById = await _context.Templates.Include(c => c.Category).FirstOrDefaultAsync(u => u.TemplateId == id);
            if(tempById == null)
            {
                TempData["Error"] = "Ingen mall hittades";
                return RedirectToAction("ViewTemplate");
            }

            return View(tempById);
        }
    }
}
