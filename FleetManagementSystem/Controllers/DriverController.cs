using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class DriverController : Controller
    {
        public IActionResult DriverPage()
        {
            return View();
        }

        public IActionResult DriverHistory()
        {
            return View();
        }
    }
}
