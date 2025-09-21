using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class MaintenanceTrackingController(ApplicationDbContext db) : Controller
    {
        private readonly ApplicationDbContext _db=db;
        //public MaintenanceController(ApplicationDbContext db)   
        //{
        //    _db = db;
        //}

        public IActionResult Maintenance_Tracking()
        {
            return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml");
        }

        public IActionResult MaintenanceEntries()
        {
            List<Models.Maintenance_Management> objMaintenanceEntries = _db.MaintenanceRecords.ToList();
            return View();
        }
        public async Task<IActionResult> AddMaintenanceRecord(Maintenance_Management obj )
        {
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("MaintenanceEntries");
        }

        public async Task<IActionResult> EditMaintenanceRecord(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var maintenacefromdb = await _db.MaintenanceRecords.FindAsync(id);
            if (maintenacefromdb == null)
            {
                return NotFound();
            }
            return View(maintenacefromdb);
        }
        [HttpPost]
        public async Task<IActionResult> EditMaintenanceRecord(Maintenance_Management obj)
        {
            _db.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("MaintenanceEntries");
        }
        public async Task<IActionResult> DeleteMaintenanceRecord(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var TripFromDb = await _db.Trips.FindAsync(id);
            if (TripFromDb == null)
            {
                return NotFound();
            }
            return View(TripFromDb);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMaintenanceRecord(Maintenance_Management obj)
        {
            _db.Remove(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("MaintenanceEntries");
        }
    }
}
