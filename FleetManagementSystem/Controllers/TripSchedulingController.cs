using System.Security.Claims;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            if (string.IsNullOrWhiteSpace(vehicleType))
                return 0;

            // Normalize: remove hyphens, trim spaces, convert to lowercase
            var normalized = vehicleType.Replace("-", "").Trim().ToLower();

            // Extract numeric part (e.g., "6wheeler" → 6)
            var match = System.Text.RegularExpressions.Regex.Match(normalized, @"(\d+)");
            if (!match.Success)
                return 0;

            int wheelCount = int.Parse(match.Value);

            // Map wheel count to capacity
            return wheelCount switch
            {
                4 => 5,
                6 => 7,
                10 => 9,
                _ => 0
            };
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally, redirect to login or home page
            return RedirectToAction("Login", "Customer");
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> AddTrip(Trip_Scheduling obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj); // Return form with validation errors
            }

            obj.Status = "Pending";
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            TempData["FleetBooked"] = true;
            TempData["SuccessMessage"] = "Fleet booked successfully!";

            return RedirectToAction("CustomerPage", "Customer"); // Or use View if needed
        }


        [HttpGet]
        public async Task<IActionResult> AddTrip()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new Trip_Scheduling
            {
                Firstname = user.FirstName,
                Lastname = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View("~/Views/Customer/CustomerPage.cshtml", model);
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
                    trip.VehicleId = driver.VehicleId;
                }

                _db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "No available driver found with matching capacity.";
            }

            return RedirectToAction("Trip_Scheduling", new { vehicleType });
        }
        [HttpPost]
        public IActionResult DeclineTrip(int tripId)
        {
            var trip = _db.Trips.FirstOrDefault(t => t.TripId == tripId);
            if (trip != null)
            {

                trip.AssignedDriver = "Declined";
                _db.SaveChanges();
            }

            return Ok();
        }

        public IActionResult TripHistory()
        {
            ViewBag.HideFooter = true;
            var trips = _db.Trips
        .Where(t => !string.IsNullOrEmpty(t.AssignedDriver) || t.AssignedDriver=="Declined")
        .ToList();
            return View("~/Views/Admin/TripScheduling/Trip_History.cshtml", trips);
        }

        public async Task<IActionResult> EditTrip(int? id)
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
        public async Task<IActionResult> EditTrip(Trip_Scheduling obj)
        {
            _db.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public int GetAcceptedTripCountForMonth(int month)
        {
            var currentYear = DateTime.Now.Year;

            int count = _db.Trips
                .Where(t => t.AssignedDriver != "Declined" &&
                            t.BookingTime.Month == month &&
                            t.BookingTime.Year == currentYear)
                .Count();

            return count;
        }

    }
}
