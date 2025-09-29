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

        public IActionResult Fuel_Management()

        {

            var records = _db.FuelRecords
                             .Include(f => f.Vehicle)
                             .ToList();

            ViewBag.SearchPerformed = false;
            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);

        }

        public IActionResult AddFuelEntries()
        {
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

            return RedirectToAction("Fuel_Management");
        }

        [HttpPost]
        public async Task<IActionResult> Fuelsearch(string registrationNumber)
        {
            List<Fuel_Management> records;

            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                // If no registration number is provided, fetch all records
                records = await _db.FuelRecords
                                   .Include(f => f.Vehicle)
                                   .ToListAsync();
            }
            else
            {
                // If a registration number is provided, filter the records
                records = await _db.FuelRecords
                                   .Include(f => f.Vehicle)
                                   .Where(f => f.Vehicle.RegistrationNumber == registrationNumber)
                                   .ToListAsync();
            }

            ViewBag.HasRecords = records.Any();
            ViewBag.SearchPerformed = true;

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);
        }




    }
}