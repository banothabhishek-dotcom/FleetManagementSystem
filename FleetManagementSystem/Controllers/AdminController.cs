using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminPage()
        {
            ViewBag.HideFooter = true;
            return View();
        }
    }
}
