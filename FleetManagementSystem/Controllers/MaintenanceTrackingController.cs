using FleetManagementSystem.Data;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
	public class MaintenanceTrackingController(ApplicationDbContext db) : Controller
	{
		private readonly ApplicationDbContext _db = db;

		public IActionResult Maintenance_Tracking()
		{
			var records = _db.MaintenanceRecords.ToList();
			return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
		}

		[HttpPost]
		public async Task<IActionResult> AddMaintenanceRecord(Maintenance_Management obj)
		{
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
	}
}