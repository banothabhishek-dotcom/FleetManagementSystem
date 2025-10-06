using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
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
        .Where(t => t.AssignedDriver != null &&
                    t.AssignedDriver.Trim().ToLower() == normalizedDriverName &&
                    t.Status == "Pending") // ✅ Only show pending trips
        .OrderByDescending(t => t.BookingTime)
        .ToList();




            ViewBag.HideFooter = true;

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

        [HttpPost]
        public async Task<IActionResult> CompleteTrip(int tripId)
        {
            var trip = await _db.Trips.FirstOrDefaultAsync(t => t.TripId == tripId);
            if (trip != null)
            {
                trip.Status = "Completed";
                // Make the vehicle available again
                var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == trip.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Status = "Available";
                }

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payment successful. Trip completed.";
            }

            return RedirectToAction("DriverPage");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally, redirect to login or home page
            return RedirectToAction("Login","Customer");
        }

        public async Task<IActionResult> DriverProfile()
        {
            ViewBag.HideFooter = true;
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View("~/Views/Driver/DriverProfile.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User_Details updatedUser)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction("Login");

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.PhoneNumber = updatedUser.PhoneNumber;

            _db.UserDetails.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile details updated!";
            return RedirectToAction("DriverProfile");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return RedirectToAction("CustomerProfile");
            }

            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var user = await _db.UserDetails.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return RedirectToAction("Login");

            var passwordHasher = new PasswordHasher<User_Details>();
            user.Password = passwordHasher.HashPassword(user, NewPassword);

            _db.UserDetails.Update(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("DriverProfile");
        }

    }
}
