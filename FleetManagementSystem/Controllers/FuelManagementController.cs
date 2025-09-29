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
            var records = _db.FuelRecords.ToList();
            ViewBag.SearchPerformed = false;
            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);

        }

        public IActionResult AddFuelEntries()
        {
            return View("~/Views/Admin/FuelManagement/AddFuelEntries.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddFuelEntries(Fuel_Management obj)
        {

            var vehicleExists = await _db.Vehicles.AnyAsync(v => v.VehicleId == obj.VehicleId);


            if (!vehicleExists)
            {
                ModelState.AddModelError("VehicleId", $"Vehicle ID {obj.VehicleId} not found.");
                return View("~/Views/Admin/FuelManagement/AddFuelEntries.cshtml", obj); // Return to the same view with error
            }

            await _db.FuelRecords.AddAsync(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Fuel_Management"); // Redirect to a page showing entries
        }

        [HttpPost]
        public async Task<IActionResult> Fuelsearch(string vehicleId)
        {
            var records = await _db.FuelRecords
                                   .Where(f => f.VehicleId.ToString() == vehicleId)
                                   .ToListAsync();

            ViewBag.HasRecords = records.Any();
            ViewBag.SearchPerformed = true;

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml", records);
        }
    }
}