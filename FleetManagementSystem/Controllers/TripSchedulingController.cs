using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class TripSchedulingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TripSchedulingController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Trip_Scheduling()
        {
            ViewBag.HideFooter = true;
            List<Models.Trip_Scheduling> objTripEntries = _db.Trips.ToList();

            var availableDrivers=_db.Vehicles
                .Where(v=>v.Status=="Available")
                .Select(v => v.DriverName)
                 .ToList();

            ViewBag.AvailableDrivers = availableDrivers;
            return View("~/Views/Admin/TripScheduling/Trip_Scheduling.cshtml",objTripEntries);
        }
        [HttpPost]
        public async Task<IActionResult> AddTrip(Trip_Scheduling obj)
        {
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return View("~/Views/Customer/CustomerPage.cshtml");
        }
        [HttpPost]
        public IActionResult AssignDriver(int tripId, string driverName)
        {
            // Find the driver in Vehicle_Management
            var driver = _db.Vehicles.FirstOrDefault(v => v.DriverName == driverName && v.Status == "Available");

            if (driver != null)
            {
                // Update driver status
                driver.Status = "Unavailable";

                // Optionally update the trip record with the assigned driver
                var trip = _db.Trips.FirstOrDefault(t => t.TripId == tripId);
                if (trip != null)
                {
                    trip.AssignedDriver = driverName;
                }

                _db.SaveChanges();
            }

            // Redirect back to the trip scheduling view
            return RedirectToAction("Trip_Scheduling");
        }

        public async Task<IActionResult> EditTrip(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            var TripFromDb = await _db.Trips.FindAsync(id);
            if (TripFromDb == null)
            {
                return NotFound();
            }
            return View(TripFromDb);
        }
        [HttpPost]
        public async Task<IActionResult> EditTrip(Trip_Scheduling obj)
        {
            _db.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTrip(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var TripFromDb = await _db.Trips.FindAsync(id);
            if (TripFromDb == null)
            {
                return NotFound();
            }
            return View(TripFromDb);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTrip(Trip_Scheduling obj)
        {
            _db.Remove(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
