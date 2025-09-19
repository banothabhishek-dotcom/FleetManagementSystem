using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
