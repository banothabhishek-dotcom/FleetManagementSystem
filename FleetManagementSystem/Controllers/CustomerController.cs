using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Login()
        {
            ViewBag.HideFooter = true;
            return View();
        }
        public IActionResult Registration()
        {
            ViewBag.HideFooter = true;
            return View();
        }
    }
}
