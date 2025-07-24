using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MallarEmelieMVC.Controllers
{
    //Så att bara admins kan nå denna ctrl
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public async Task<IActionResult> CreateTemplate()
        {
            return View();
        }
    }
}
