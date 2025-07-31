using MallarEmelieMVC.Data;
using MallarEmelieMVC.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MallarEmelieMVC.Controllers
{
    //Så att bara admins kan nå denna ctrl
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet] //för varje post metod behöver vi en getmetod, då postmetod tar emot ett formulär, och det är genom denna getmetod vi visar formen.
        public async Task<IActionResult> CreateTemplate()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate(Template template, IFormFile ImageUpload)
        {
            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templatePics");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(stream);
                }

                template.ImageUrl = "/templatePics/" + uniqueFileName;
            }

            if (template.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Du måste välja en kategori.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vänligen fyll i alla obligatoriska fält.";
                return View(template);
            }

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mallen har lagts till!";
            return RedirectToAction("CreateTemplate");
        }

        [HttpGet]
        public async Task<IActionResult> EditTemplate(int id)
        {
            var tempById = await _context.Templates.Include(c => c.Category).FirstOrDefaultAsync(t => t.TemplateId == id);

            if(tempById == null)
            {
                TempData["Error"] = "Kunde ej hitta mallen";
                return RedirectToAction("ViewTemplate");
            }
            return View(tempById);
        }

        [HttpPost]
        public async Task<IActionResult> EditTemplate(Template updatedTemplate, IFormFile? ImageUpload)
        {

            var existingTemplate = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateId == updatedTemplate.TemplateId);

            if (existingTemplate == null)
            {
                TempData["Error"] = "Mallen kunde inte hittas.";
                return RedirectToAction("ViewTemplate");
            }

            existingTemplate.Title = updatedTemplate.Title;
            existingTemplate.Description = updatedTemplate.Description;
            existingTemplate.Price = updatedTemplate.Price;
            existingTemplate.CategoryId = updatedTemplate.CategoryId;

            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templatePics");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(stream);
                }

                existingTemplate.ImageUrl = "/templatePics/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Mallen har uppdaterats!";
            return RedirectToAction("ViewTemplate", "Template");
           
        }
    }
}
