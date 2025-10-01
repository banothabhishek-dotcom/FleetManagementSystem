using FleetManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DriverController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> DriverPage()
        {
            var driverName = HttpContext.Session.GetString("DriverName");

            if (string.IsNullOrEmpty(driverName))
            {
                var email = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Login");
                }

                var driver = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
                if (driver == null)
                {
                    return RedirectToAction("Login");
                }

                driverName = $"{driver.FirstName?.Trim()} {driver.LastName?.Trim()}";
                HttpContext.Session.SetString("DriverName", driverName);
            }

            var normalizedDriverName = driverName.Trim().ToLower();

            var assignedTrips = _db.Trips
                .Where(t => t.AssignedDriver != null && t.AssignedDriver.Trim().ToLower() == normalizedDriverName)
                .OrderByDescending(t => t.BookingTime)
                .ToList();

            return View("~/Views/Driver/DriverPage.cshtml", assignedTrips);
        }





        public async Task<IActionResult> DriverHistory()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Customer");
            }

            var driver = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
            if (driver == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            var fullName = $"{driver.FirstName?.Trim()} {driver.LastName?.Trim()}".ToLower();

            var history = _db.Trips
                .Where(t => t.AssignedDriver != null && t.AssignedDriver.Trim().ToLower() == fullName)
                .OrderByDescending(t => t.BookingTime)
                .ToList();

            ViewBag.HideFooter = true;

            return View("~/Views/Driver/DriverHistory.cshtml", history);
        }

    }
}
