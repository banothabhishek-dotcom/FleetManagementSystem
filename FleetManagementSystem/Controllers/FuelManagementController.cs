using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class FuelManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
