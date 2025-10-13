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

            // Query fuel records and include related vehicle data 
            var fuelQuery = _db.FuelRecords
                               .Include(f => f.Vehicle);

            int totalRecords = fuelQuery.Count();

            // Calculate total number of pages needed
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Fetch only the records for the current page, sorted by date (latest first)
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
            // Look up the vehicle by registration number, ignoring deleted ones 
            //in vehicle table if isdeleted is 1 is nothing but deleted that record
            var vehicle = await _db.Vehicles
                                   .FirstOrDefaultAsync(v => v.RegistrationNumber == RegistrationNumber && !v.IsDeleted);

            if (vehicle == null)
            {
                ModelState.AddModelError("RegistrationNumber", $"Vehicle with registration number '{RegistrationNumber}' not found.");
                return View("~/Views/Admin/FuelManagement/AddFuelEntries.cshtml", obj);
            }

            // Assign the found vehicle's ID to the fuel entry
            obj.VehicleId = vehicle.VehicleId;

            await _db.FuelRecords.AddAsync(obj);
            await _db.SaveChangesAsync();

            return RedirectToAction("Fuel_Management", new { page = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Fuelsearch(string registrationNumber, int page = 1)
        {
            ViewBag.HideFooter = true;
            int pageSize = 10;
            IQueryable<Fuel_Management> query = _db.FuelRecords.Include(f => f.Vehicle);

            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                query = query.Where(f => f.Vehicle.RegistrationNumber == registrationNumber);
            }

            // Count total matching records for pagination
            int totalRecords = await query.CountAsync();

            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Fetch only the records for the current page, sorted by date (latest first)
            var records = await query
                .OrderByDescending(f => f.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.HasRecords = records.Any(); // True if any records were found
            ViewBag.SearchPerformed = true;// Indicates a search was done
            ViewBag.CurrentPage = page;// Current page number
            ViewBag.TotalPages = totalPages;// Total number of pages
            ViewBag.SearchQuery = registrationNumber;// Preserve search input

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);
        }




    }
}