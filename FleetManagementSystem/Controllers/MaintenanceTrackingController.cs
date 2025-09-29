using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
	public class MaintenanceTrackingController(ApplicationDbContext db) : Controller
	{
		private readonly ApplicationDbContext _db = db;

		public IActionResult Maintenance_Tracking(int page = 1)
		{
			int pageSize = 5;
			var records = _db.MaintenanceRecords
							 .OrderByDescending(m => m.ScheduledDate)
							 .Skip((page - 1) * pageSize)
							 .Take(pageSize)
							 .ToList();

			int totalRecords = _db.MaintenanceRecords.Count();
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.SearchPerformed = false;

			return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
		}

		[HttpPost]
		public async Task<IActionResult> AddMaintenanceRecord(Maintenance_Management obj)
		{
			// Check if the vehicle exists
			var vehicleExists = await _db.Vehicles.AnyAsync(v => v.VehicleId == obj.VehicleId);
			if (!vehicleExists)
			{
				// Add validation error
				ModelState.AddModelError("VehicleID", $"Vehicle ID {obj.VehicleId} not found.");

				// Fetch paginated maintenance records
				int pageSize = 5;
				int page = 1;
				var records = _db.MaintenanceRecords
								 .OrderByDescending(m => m.ScheduledDate)
								 .Skip((page - 1) * pageSize)
								 .Take(pageSize)
								 .ToList();

				int totalRecords = _db.MaintenanceRecords.Count();
				ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
				ViewBag.CurrentPage = page;

				// Return the same view with the list and error
				return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
			}

			// Save the new record
			obj.Status = "Scheduled";
			await _db.MaintenanceRecords.AddAsync(obj);
			await _db.SaveChangesAsync();

			return RedirectToAction("Maintenance_Tracking");
		}

		public async Task<IActionResult> DeleteMaintenanceRecord(int id)
		{
			var record = await _db.MaintenanceRecords.FindAsync(id);
			if (record == null)
			{
				return NotFound();
			}

			_db.MaintenanceRecords.Remove(record);
			await _db.SaveChangesAsync();
			return RedirectToAction("Maintenance_Tracking");
		}

		public async Task<IActionResult> MarkAsComplete(int id)
		{
			var record = await _db.MaintenanceRecords.FindAsync(id);
			if (record != null)
			{
				record.Status = "Completed";
				await _db.SaveChangesAsync();
			}
			return RedirectToAction("Maintenance_Tracking");
		}
		[HttpPost]
		public async Task<IActionResult> Maintenancesearch(string vehicleId)
		{
			var records = await _db.MaintenanceRecords
								   .Where(f => f.VehicleId.ToString() == vehicleId)
								   .ToListAsync();

			ViewBag.HasRecords = records.Any();
			ViewBag.SearchPerformed = true;

			return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
		}
	}
}