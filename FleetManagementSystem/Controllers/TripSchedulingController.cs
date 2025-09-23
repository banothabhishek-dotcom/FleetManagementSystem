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
        private int GetRequiredCapacity(string vehicleType)
        {
            return vehicleType switch
            {
                "4-Wheeler" => 5,
                "6-Wheeler" => 7,
                "10-Wheeler" => 9,
                _ => 0
            };
        }


        public IActionResult Trip_Scheduling()
        {
            ViewBag.HideFooter = true;
            //List<Models.Trip_Scheduling> objTripEntries = _db.Trips.ToList();

            List<Models.Trip_Scheduling> objTripEntries = _db.Trips
       .Where(t => string.IsNullOrEmpty(t.AssignedDriver))
       .ToList();

            var availableVehicles = _db.Vehicles
                .Where(v => v.Status == "Available")
                .ToList();

            ViewBag.AvailableVehicles = availableVehicles;

            return View("~/Views/Admin/TripScheduling/Trip_Scheduling.cshtml", objTripEntries);
        }




        public async Task<IActionResult> AddTrip(Trip_Scheduling obj)
        {
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return View();
        }
        [HttpPost]
        public IActionResult AssignDriver(int tripId, string driverName, string vehicleType)
        {
            int requiredCapacity = GetRequiredCapacity(vehicleType);

            if (requiredCapacity == 0)
            {
                TempData["Error"] = "Unsupported vehicle type.";
                return RedirectToAction("Trip_Scheduling", new { vehicleType });
            }

            var driver = _db.Vehicles.FirstOrDefault(v =>
                v.DriverName == driverName &&
                v.Status == "Available" &&
                v.Capacity == requiredCapacity);

            if (driver != null)
            {
                driver.Status = "Unavailable";

                var trip = _db.Trips.FirstOrDefault(t => t.TripId == tripId);
                if (trip != null)
                {
                    trip.AssignedDriver = driverName;
                }

                _db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "No available driver found with matching capacity.";
            }

            return RedirectToAction("Trip_Scheduling", new { vehicleType });
        }


        [HttpGet]
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
