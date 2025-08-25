using MallarEmelieMVC.Areas.Identity.Models;
using MallarEmelieMVC.Data;
using MallarEmelieMVC.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [HttpGet]
        public async Task<IActionResult> RemoveTemplate(int id)
        {
            var temp = await _context.Templates.Include(t => t.Category).FirstOrDefaultAsync(t => t.TemplateId == id);
            if(temp == null)
            {
                TempData["Error"] = "Ingen mall hittades med det ID:t.";
                return RedirectToAction("ViewTemplate", "Template");
            }

            return View(temp);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmRemoveTemplate(int id)
        {
            var deleteTemp = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if(deleteTemp == null)
            {
                TempData["Error"] = "Ingen mall hittades med det Id:t";
                return RedirectToAction("ViewTemplate", "Template");
            }

            _context.Templates.Remove(deleteTemp);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Mallen raderades!";
            return RedirectToAction("ViewTemplate", "Template");
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Template)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
         
            if(order == null)
            {
                TempData["Error"] = "Ingen order hittades med det Id:t";
                return RedirectToAction("SearchOrder", "Admin");
            }


            return View(order);
        }


        //så admin kan söka upp alla orders genom orderid

        [HttpGet]
        public async Task<IActionResult> SearchOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Template)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if(order == null)
            {
                TempData["Error"] = "Ingen order hittades med det Id:t";
                return RedirectToAction("SearchOrder", "Admin");
            }

            return RedirectToAction("ViewOrder", new { orderId = order.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if(order == null)
            {
                TempData["Error"] = "Ordern kunde inte hittas.";
                return RedirectToAction("SearchOrder");
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(Order updateOrder)
        {
            var existingOrder = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == updateOrder.OrderId);
            if(existingOrder == null)
            {
                TempData["Error"] = "Ordern kunde inte hittas.";
                return RedirectToAction("SearchOrder");
            }
            //personupgftr
            existingOrder.FirstName = updateOrder.FirstName;
            existingOrder.LastName = updateOrder.LastName;
            existingOrder.Status = updateOrder.Status;
            existingOrder.MobileNr = updateOrder.MobileNr;
            existingOrder.StreetAddress = updateOrder.StreetAddress;
            existingOrder.PostalCode = updateOrder.PostalCode;
            existingOrder.City = updateOrder.City;
            existingOrder.Status = updateOrder.Status;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Ordern har uppdaterats!";
            //skickar med idt, för att kunna se medd på samma sida med updt värden
            return RedirectToAction("EditOrder", new { orderId = updateOrder.OrderId });
        }

        //För o enkelt kunna ändra order status manuellt genom admin kontot
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if(order == null)
            {
                TempData["Error"] = "Ordern kunde inte hittas.";
                return RedirectToAction("SearchOrder");
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Status uppdaterad!";
            return RedirectToAction("ViewOrder", new { orderId });
        }
    }
}
