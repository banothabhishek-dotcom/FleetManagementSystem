using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace FleetManagementSystem.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VehicleManagementController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult VehicleEntries()
        {
            //to list the tables we have to write listing logic here 
            //dbset needs to be added
            List<Models.Vehicle_Management> objVehicleEntries = _db.Vehicle_Management.ToList();
            return View(objVehicleEntries);
        }
        [HttpPost]
        public IActionResult AddVehicle(Vehicle_Management obj)
        {
            _db.AddAsync(obj);
            _db.SaveChangesAsync();
            return RedirectToAction("VehicleEntries");
        }
        [HttpGet]
        public async Task<IActionResult> EditVehicle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = _db.Vehicle_Management.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View(vehicleFromDb);
        }
        [HttpPost]
        public IActionResult EditVehicle(Vehicle_Management obj)
        {
            _db.Vehicle_Management.Update(obj);
            _db.SaveChangesAsync();
            return RedirectToAction("VehicleEntries");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVehicleAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = await _db.Vehicle_Management.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View(vehicleFromDb);
        }
        [HttpPost, ActionName("DeleteVehicle")]
        public IActionResult DeleteVehiclePOST(int? id)
        {
            var obj = _db.Vehicle_Management.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Vehicle_Management.Remove(obj);
            _db.SaveChangesAsync();
            return RedirectToAction("VehicleEntries");
        }
    }
}
