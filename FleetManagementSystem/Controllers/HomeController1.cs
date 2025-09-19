using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
