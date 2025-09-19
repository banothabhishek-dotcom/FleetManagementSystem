using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class TripSchedulingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
