using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly ApplicationDbContext _db;
        public VehicleManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        //public IActionResult Vehicle_Management()
        //{
        //    //to list the tables we have to write listing logic here 
        //    //dbset needs to be added
        //    ViewBag.HideFooter = true;

        //    var vehicles = _db.Vehicles
        //            .Include(v => v.MaintenanceRecords)
        //            .ToList();

        //    // Compute LastServicedDate from related MaintenanceRecords
        //    foreach (var vehicle in vehicles)
        //    {
        //        var latestService = vehicle.MaintenanceRecords
        //            .OrderByDescending(m => m.ScheduledDate)
        //            .FirstOrDefault();

        //        vehicle.LastServicedDate = latestService?.ScheduledDate;
        //    }

        //    List<Models.Vehicle_Management> objVehicleEntries = _db.Vehicles.ToList();
        //    return View("~/Views/Admin/VehicleManagement/Vehicle_Management.cshtml", objVehicleEntries);

        //}

        //public IActionResult Vehicle_Management()
        //{
        //    ViewBag.HideFooter = true;

        //    var vehicles = _db.Vehicles
        //        .Where(v => !v.IsDeleted)
        //        .Include(v => v.MaintenanceRecords)
        //        .ToList();

        //    foreach (var vehicle in vehicles)
        //    {
        //        var latestService = vehicle.MaintenanceRecords
        //            .OrderByDescending(m => m.ScheduledDate)
        //            .FirstOrDefault();

        //        vehicle.LastServicedDate = latestService?.ScheduledDate;
        //    }

        //    return View("~/Views/Admin/VehicleManagement/Vehicle_Management.cshtml", vehicles);
        //}
        public IActionResult Vehicle_Management(int page = 1)
        {
            ViewBag.HideFooter = true;
            int pageSize = 10;

            var vehiclesQuery = _db.Vehicles
                .Where(v => !v.IsDeleted)
                .Include(v => v.MaintenanceRecords);

            int totalVehicles = vehiclesQuery.Count();
            int totalPages = (int)Math.Ceiling(totalVehicles / (double)pageSize);

            var vehicles = vehiclesQuery
                .OrderBy(v => v.VehicleId) // Ensure consistent ordering
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var vehicle in vehicles)
            {
                var latestService = vehicle.MaintenanceRecords
                    .OrderByDescending(m => m.ScheduledDate)
                    .FirstOrDefault();

                vehicle.LastServicedDate = latestService?.ScheduledDate;
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("~/Views/Admin/VehicleManagement/Vehicle_Management.cshtml", vehicles);
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
        //[HttpPost]
        //public async Task<IActionResult> Delete_Vehicle(int? id, Vehicle_Management obj)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    // Step 1: Unmap vehicleId in Maintenance table
        //    var maintenanceRecords = _db.MaintenanceRecords
        //        .Where(m => m.VehicleId == id)
        //        .ToList();

        //    foreach (var record in maintenanceRecords)
        //    {
        //        record.VehicleId = null;
        //    }
        //    _db.MaintenanceRecords.UpdateRange(maintenanceRecords);

        //    // Step 2: Unmap vehicleId in other related tables (if any)
        //    // Repeat similar logic for Fuel_Management, Trip_Scheduling, etc., if needed

        //    // Step 3: Delete the vehicle
        //    var vehicle = await _db.Vehicles.FindAsync(id);
        //    if (vehicle == null)
        //    {
        //        return NotFound();
        //    }

        //    _db.Vehicles.Remove(vehicle);
        //    await _db.SaveChangesAsync();

        //    return RedirectToAction("Vehicle_Management");
        //}
        [HttpPost]
        public async Task<IActionResult> Delete_Vehicle(int? id, Vehicle_Management obj)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            // Soft delete
            vehicle.IsDeleted = true;
            _db.Vehicles.Update(vehicle);
            await _db.SaveChangesAsync();

            return RedirectToAction("Vehicle_Management");
        }


    }
}
