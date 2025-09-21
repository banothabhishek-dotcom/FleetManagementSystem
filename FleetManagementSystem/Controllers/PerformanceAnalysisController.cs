using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class PerformanceAnalysisController : Controller
    {
        public IActionResult Performance_Analysis()
        {
            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml");
        }
    }
}
