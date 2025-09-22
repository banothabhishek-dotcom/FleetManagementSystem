using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace FleetManagementSystem.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VehicleManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Vehicle_Management()
        {
            //to list the tables we have to write listing logic here 
            //dbset needs to be added
            ViewBag.HideFooter = true;
            List<Models.Vehicle_Management> objVehicleEntries = _db.Vehicles.ToList();
            return View("~/Views/Admin/VehicleManagement/Vehicle_Management.cshtml", objVehicleEntries);

        }
        public IActionResult Add_Vehicle()
        {
            return View("~/Views/Admin/VehicleManagement/Add_Vehicle.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Add_Vehicle(Vehicle_Management obj)
        {
            await _db.AddAsync(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Vehicle_Management");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Vehicle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = await _db.Vehicles.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/VehicleManagement/Edit_Vehicle.cshtml", vehicleFromDb);
        }


        [HttpPost]
        public async Task<IActionResult> Edit_Vehicle(int? id, Vehicle_Management obj)
        {

            if (id == null || id == 0 || obj.VehicleId != id)
            {
                return NotFound();
            }


            _db.Vehicles.Update(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Vehicle_Management");
        }
        [HttpGet]
        public async Task<IActionResult> Delete_Vehicle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var vehicleFromDb = await _db.Vehicles.FindAsync(id);
            if (vehicleFromDb == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/VehicleManagement/Delete_Vehicle.cshtml",vehicleFromDb);
        }
        [HttpPost]
        public async Task<IActionResult> Delete_Vehicle(int? id, Vehicle_Management obj)
        {
            if (id == null)
            {
                return NotFound();
            }
            _db.Vehicles.Remove(obj);
            await _db.SaveChangesAsync();
            return RedirectToAction("Vehicle_Management");
        }
    }
}
