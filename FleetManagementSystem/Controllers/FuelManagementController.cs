using FleetManagementSystem.Data;
using FleetManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers

{

    public class FuelManagementController : Controller

    {

        private readonly ApplicationDbContext _db;

        public FuelManagementController(ApplicationDbContext db)

        {

            _db = db;

        }
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally, redirect to login or home page
            return RedirectToAction("Login", "Customer");
        }

        public IActionResult Fuel_Management(int page = 1)

        {
            ViewBag.HideFooter = true;
            int pageSize = 10;

            var fuelQuery = _db.FuelRecords
                               .Include(f => f.Vehicle);

            int totalRecords = fuelQuery.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var records = fuelQuery
                          .OrderByDescending(f => f.Date)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchPerformed = false;

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);
        }

        public IActionResult AddFuelEntries()
        {
            ViewBag.HideFooter = true;
            return View("~/Views/Admin/FuelManagement/AddFuelEntries.cshtml");
        }


        [HttpPost]
        public async Task<IActionResult> AddFuelEntries(Fuel_Management obj, string RegistrationNumber)
        {
            var vehicle = await _db.Vehicles
                                   .FirstOrDefaultAsync(v => v.RegistrationNumber == RegistrationNumber && !v.IsDeleted);

            if (vehicle == null)
            {
                ModelState.AddModelError("RegistrationNumber", $"Vehicle with registration number '{RegistrationNumber}' not found.");
                return View("~/Views/Admin/FuelManagement/AddFuelEntries.cshtml", obj);
            }

            obj.VehicleId = vehicle.VehicleId;

            await _db.FuelRecords.AddAsync(obj);
            await _db.SaveChangesAsync();

            return RedirectToAction("Fuel_Management", new { page = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Fuelsearch(string registrationNumber, int page = 1)
        {
            int pageSize = 10;
            IQueryable<Fuel_Management> query = _db.FuelRecords.Include(f => f.Vehicle);

            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                query = query.Where(f => f.Vehicle.RegistrationNumber == registrationNumber);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var records = await query
                .OrderByDescending(f => f.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.HasRecords = records.Any();
            ViewBag.SearchPerformed = true;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = registrationNumber;

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);
        }
       




    }
}