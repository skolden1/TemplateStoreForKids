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
            if (!ModelState.IsValid)
            {
                return View(template);
            }

            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                //för o hitta till mappen templatepics som ligger i wwwroot
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templatePics");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // filnamn
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(stream);
                }

                
                template.ImageUrl = "/templatePics/" + uniqueFileName;
            }

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Mallen har lagts till!";
            return RedirectToAction("CreateTemplate");
        }
    }
}
