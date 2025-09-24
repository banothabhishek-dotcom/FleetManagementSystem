using FleetManagementSystem.Data;

using FleetManagementSystem.Models;

using Microsoft.AspNetCore.Mvc;

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

            return View("~/Views/Admin/FuelManagement/Fuel_Management.cshtml");

        }

        public IActionResult FuelEntries()

        {

            List<Models.Fuel_Management> objFuelEntries = _db.FuelRecords.ToList();

            return View();

        }




        [HttpPost]
        public async Task<IActionResult> AddFuelEntries(Fuel_Management obj)
        {
             // Return the view with validation errors

            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Fuel_Management"); // Redirect to a page showing entries
        }





        [HttpGet]

        public async Task<IActionResult> EditFuelEntries(int? id)

        {

            if (id == null || id == 0)

            {

                return NotFound();

            }

            var fuelFromDb = await _db.FuelRecords.FindAsync(id);

            if (fuelFromDb == null)

            {

                return NotFound();

            }

            return View(fuelFromDb);

        }

        [HttpPost]

        public async Task<IActionResult> EditFuelEntries(Fuel_Management obj)

        {

            _db.FuelRecords.Update(obj);

            await _db.SaveChangesAsync();

            return RedirectToAction("FuelEntries");

        }

        public async Task<IActionResult> DeleteFuelEntries(int? id)

        {

            if (id == null || id == 0)

            {

                return NotFound();

            }

            var fuelFromDb = await _db.FuelRecords.FindAsync(id);

            if (fuelFromDb == null)

            {

                return NotFound();

            }

            return View(fuelFromDb);

        }

        [HttpPost]

        public async Task<IActionResult> DeleteFuelEntries(Fuel_Management obj)

        {

            _db.FuelRecords.Remove(obj);

            await _db.SaveChangesAsync();

            return RedirectToAction("FuelEntries");

        }

    }

}

