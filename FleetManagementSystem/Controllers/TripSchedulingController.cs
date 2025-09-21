using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class TripSchedulingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TripSchedulingController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Trip_Scheduling()
        {
            return View("~/Views/Admin/TripScheduling/Trip_Scheduling.cshtml");
        }

        public async Task<IActionResult> AddTrip(Trip_Scheduling obj)
        {
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return View();
        }

        public async Task<IActionResult> EditTrip(int? id)
        {
            if(id==null || id == 0)
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
        public async Task<IActionResult> EditTrip(Trip_Scheduling obj)
        {
            _db.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteTrip(int? id)
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
        public async Task<IActionResult> DeleteTrip(Trip_Scheduling obj)
        {
            _db.Remove(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
