using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class PerformanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
