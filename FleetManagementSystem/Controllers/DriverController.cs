using FleetManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DriverController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult DriverPage()
        {
            var driverName = HttpContext.Session.GetString("DriverName");
            if (string.IsNullOrEmpty(driverName))
            {
                return RedirectToAction("Login");
            }

            var assignedTrips = _db.Trips
                .Where(t => t.AssignedDriver == driverName)
                .OrderByDescending(t => t.BookingTime)
                .ToList();

            return View("~/Views/Driver/DriverPage.cshtml", assignedTrips);
        }



        public IActionResult DriverHistory()
        {
            return View();
        }
    }
}
