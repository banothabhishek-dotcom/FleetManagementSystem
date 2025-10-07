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

            var normalized = vehicleType.Replace("-", "").Trim().ToLower();
            var match = System.Text.RegularExpressions.Regex.Match(normalized, @"(\d+)");
            if (!match.Success)
                return 0;

            int wheelCount = int.Parse(match.Value);

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

            // Get trips that still need driver assignment
            var objTripEntries = _db.Trips
                .Where(t => string.IsNullOrEmpty(t.AssignedDriver))
                .ToList();

            
            var availableVehicles = _db.Vehicles.ToList(); // Include all vehicles


            // Get all trips for conflict checking
            var allTrips = _db.Trips
                .Where(t => !string.IsNullOrEmpty(t.AssignedDriver))
                .ToList();

            ViewBag.AvailableVehicles = availableVehicles;
            ViewBag.AllTrips = allTrips;

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
                return BadRequest();

            var trip = _db.Trips.FirstOrDefault(t => t.TripId == tripId);
            if (trip == null)
                return NotFound();

            bool isDriverBusy = _db.Trips.Any(t =>
                t.BookingTime.Date == trip.BookingTime.Date &&
                t.AssignedDriver == driverName &&
                t.TripId != tripId);

            if (isDriverBusy)
                return Conflict();

            var driverVehicle = _db.Vehicles.FirstOrDefault(v =>
                v.DriverName == driverName &&
                v.Capacity == requiredCapacity);

            if (driverVehicle == null)
                return NotFound();

            trip.AssignedDriver = driverName;
            trip.VehicleId = driverVehicle.VehicleId;

            _db.SaveChanges();

            return Ok(); // ✅ AJAX expects this
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
