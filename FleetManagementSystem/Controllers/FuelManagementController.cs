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
        public IActionResult FuelEntries()
        {
            List<Models.Fuel_Management> objFuelEntries = _db.FuelRecords.ToList();
            return View(objFuelEntries);
        }


        [HttpPost]
        public IActionResult AddFuelEntries(Fuel_Management obj)
        {
            _db.AddAsync(obj);
            _db.SaveChangesAsync();
            return RedirectToAction("FuelEntries");
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
        public IActionResult EditFuelEntries(Fuel_Management obj)
        {
            _db.FuelRecords.Update(obj);
            _db.SaveChangesAsync();
            return RedirectToAction("FuelEntries");
        }

    }
}
