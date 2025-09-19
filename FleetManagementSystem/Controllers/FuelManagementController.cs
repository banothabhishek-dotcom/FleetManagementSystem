using FleetManagementSystem.Data;
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


    }
}
