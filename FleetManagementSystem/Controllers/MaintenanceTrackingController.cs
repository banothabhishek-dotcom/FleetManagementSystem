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
			ViewBag.HideFooter = true;
			int pageSize = 5;
			var records = _db.MaintenanceRecords
				 .Include(m => m.Vehicle)
								 .Where(m => m.Vehicle != null && !m.Vehicle.IsDeleted)
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
		public async Task<IActionResult> AddMaintenanceRecord(string RegistrationNumber, DateTime ScheduledDate, string Description)
		{
			// Find the vehicle by registration number
			var vehicle = await _db.Vehicles
				.FirstOrDefaultAsync(v => v.RegistrationNumber == RegistrationNumber && !v.IsDeleted);

			if (vehicle == null)
			{
				ModelState.AddModelError("RegistrationNumber", $"Vehicle with registration number '{RegistrationNumber}' not found.");

				// Re-fetch maintenance records for the view
				int pageSize = 5;
				int page = 1;
				var records = await _db.MaintenanceRecords
					.Include(m => m.Vehicle)
					.Where(m => m.Vehicle != null && !m.Vehicle.IsDeleted)
					.OrderByDescending(m => m.ScheduledDate)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				ViewBag.TotalPages = (int)Math.Ceiling((double)await _db.MaintenanceRecords.CountAsync() / pageSize);
				ViewBag.CurrentPage = page;

				return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
			}

			// Create and save the maintenance record
			var newRecord = new Maintenance_Management
			{
				VehicleId = vehicle.VehicleId,
				ScheduledDate = ScheduledDate,
				Description = Description,
				Status = "Scheduled"
			};

			await _db.MaintenanceRecords.AddAsync(newRecord);
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
		public async Task<IActionResult> Maintenancesearch(string registrationNumber)
		{
			ViewBag.HideFooter = true;
			List<Maintenance_Management> records;

			if (string.IsNullOrWhiteSpace(registrationNumber))
			{
				// If no registration number is provided, fetch all records
				records = await _db.MaintenanceRecords
								   .Include(m => m.Vehicle)
								   .Where(m => m.Vehicle != null && !m.Vehicle.IsDeleted)
								   .ToListAsync();
			}
			else
			{
				// If a registration number is provided, filter the records
				records = await _db.MaintenanceRecords
								   .Include(m => m.Vehicle)
								   .Where(m => m.Vehicle != null &&
											   !m.Vehicle.IsDeleted &&
											   m.Vehicle.RegistrationNumber == registrationNumber)
								   .ToListAsync();
			}

			ViewBag.HasRecords = records.Any();
			ViewBag.SearchPerformed = true;

			return View("~/Views/Admin/MaintenanceTracking/Maintenance_Tracking.cshtml", records);
		}

		[HttpPost]
		public IActionResult Logout()
		{
			// Clear session
			HttpContext.Session.Clear();

			// Redirect to login page
			return RedirectToAction("Login", "Admin"); // or "Account" depending on your setup
		}

	}
}