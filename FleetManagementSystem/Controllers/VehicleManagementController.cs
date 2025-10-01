using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        private string phoneNumber;
        private readonly IPasswordHasher<User_Details> _passwordHasher;

        

        public VehicleManagementController(ApplicationDbContext db, IPasswordHasher<User_Details> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

       
        public IActionResult Vehicle_Management(int page = 1)
        {
            ViewBag.HideFooter = true;
            int pageSize = 10;

            var vehiclesQuery = _db.Vehicles
                .Where(v => !v.IsDeleted)
                .Include(v => v.MaintenanceRecords);

            int totalVehicles = vehiclesQuery.Count();
            int totalPages = (int)Math.Ceiling(totalVehicles / (double)pageSize);

            var vehicles = vehiclesQuery
                .OrderBy(v => v.VehicleId) // Ensure consistent ordering
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var vehicle in vehicles)
            {
                var latestService = vehicle.MaintenanceRecords
                    .OrderByDescending(m => m.ScheduledDate)
                    .FirstOrDefault();

                vehicle.LastServicedDate = latestService?.ScheduledDate;
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("~/Views/Admin/VehicleManagement/Vehicle_Management.cshtml", vehicles);
        }



        public IActionResult Add_Vehicle()
        {
            return View("~/Views/Admin/VehicleManagement/Add_Vehicle.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Add_Vehicle(Vehicle_Management obj)
        {

            if (ModelState.IsValid)
            {
                // Save vehicle details
                await _db.Vehicles.AddAsync(obj);

                // Extract first name and phone number from DriverName
                var nameParts = obj.DriverName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                string firstName = nameParts.Length > 0 ? nameParts[0] : obj.DriverName;
                string lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                

                // Create user details
                var user = new User_Details
                {
                    FirstName = firstName,
                    LastName=lastName,
                    PhoneNumber = obj.DriverPhone,
                    Email = $"{obj.DriverPhone}@fleet.com",
                    Password = "Driver@123", // ⚠️ Hash this in production!
                    Role = "Driver"
                };

                user.Password = _passwordHasher.HashPassword(user, "Driver@123");
                // Save user details
                await _db.UserDetails.AddAsync(user);

                // Commit both changes
                await _db.SaveChangesAsync();

                return RedirectToAction("Vehicle_Management");
            }

            return View(obj);
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Vehicle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = await _db.Vehicles.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/VehicleManagement/Edit_Vehicle.cshtml", vehicleFromDb);
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Vehicle(int? id, Vehicle_Management obj)
        {

            if (id == null || id == 0 || obj.VehicleId != id)
            {
                return NotFound();
            }


            _db.Vehicles.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Vehicle_Management");
        }
        [HttpGet]
        public async Task<IActionResult> Delete_Vehicle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = await _db.Vehicles.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/VehicleManagement/Delete_Vehicle.cshtml",vehicleFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Delete_Vehicle(int? id, Vehicle_Management obj)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            // Soft delete
            vehicle.IsDeleted = true;
            _db.Vehicles.Update(vehicle);
            await _db.SaveChangesAsync();

            return RedirectToAction("Vehicle_Management");
        }


    }
}
