using FleetManagementSystem.Data;
using FleetManagementSystem.Helpers;
using FleetManagementSystem.Models;
using iText.Commons.Actions.Contexts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Controllers
{
    public class PerformanceAnalysisController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PerformanceAnalysisController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally, redirect to login or home page
            return RedirectToAction("Login","Customer");
        }
        public IActionResult Performance_Analysis()

        {
            ViewBag.HideFooter = true;
            var monthlyAcceptedTrips = GetMonthlyAcceptedTrips();
            var monthlyFuelData = GetMonthlyFuelData();
            var acceptedTripsChartBytes = ChartGenerator.GenerateAcceptedTripsChart(monthlyAcceptedTrips);
            var fuelChartBytes = FuelChartGenerator.GenerateFuelBarChart(monthlyFuelData);
            var stats = GetFleetStats(monthlyAcceptedTrips);

            var model = new Performance_Analysis
            {
                TotalTrips = stats.TotalTrips
            };

            ViewBag.ChartImage = Convert.ToBase64String(acceptedTripsChartBytes);
            ViewBag.FuelChartImage = Convert.ToBase64String(fuelChartBytes);
            ViewBag.MonthlyData = monthlyAcceptedTrips;
            ViewBag.AvailableVehicles = stats.AvailableVehicles;
            ViewBag.UnavailableVehicles = stats.UnavailableVehicles;
            ViewBag.Scheduled = stats.ScheduledMaintenance;
            ViewBag.Completed = stats.CompletedMaintenance;

            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPerformancePdf()
        {
            var monthlyAcceptedTrips = GetMonthlyAcceptedTrips();
            var monthlyFuelData = GetMonthlyFuelData();
            var acceptedTripsChartBytes = ChartGenerator.GenerateAcceptedTripsChart(monthlyAcceptedTrips);
            var fuelChartBytes = FuelChartGenerator.GenerateFuelBarChart(monthlyFuelData);
            var stats = GetFleetStats(monthlyAcceptedTrips);

            // ✅ Insert into Performance table
            var report = new Performance_Analysis
            {
                ReportType = "pdf",
                Data = $"Total trips: {stats.TotalTrips}\n" +
                $" Scheduled maintenance: {stats.ScheduledMaintenance}\n" +
                $" Completed maintenance: {stats.CompletedMaintenance}\n" +
                $" Available vehicles: {stats.AvailableVehicles}\n " +
                $"Unavailable vehicles: {stats.UnavailableVehicles}",
                GeneratedOn = DateTime.UtcNow
            };

            _db.PerformanceReports.Add(report);
            await _db.SaveChangesAsync();

            // ✅ Generate PDF
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph("Fleet Performance Summary"));
                doc.Add(new Paragraph($"Total Trips This Year: {stats.TotalTrips}"));
                doc.Add(new Paragraph($"Maintenance Records - Scheduled: {stats.ScheduledMaintenance}, Completed: {stats.CompletedMaintenance}"));
                doc.Add(new Paragraph($"Vehicle Status - Available: {stats.AvailableVehicles}, Unavailable: {stats.UnavailableVehicles}"));
                doc.Add(new Paragraph(" "));

                iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(acceptedTripsChartBytes);
                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(fuelChartBytes);

                img1.ScaleToFit(500f, 300f);
                img2.ScaleToFit(500f, 300f);

                doc.Add(new Paragraph("Accepted Trips Overview"));
                doc.Add(img1);
                doc.Add(new Paragraph("Fuel Consumption Overview"));
                doc.Add(img2);

                doc.Close();
                return File(ms.ToArray(), "application/pdf", "PerformanceGraphs.pdf");
            }
        }
        [HttpPost]
        private async Task<IActionResult> DownloadReport()
        {
            var report = new Performance_Analysis
            {
                ReportType = "pdf",
                Data = "some info", // Replace with actual report summary or JSON
                GeneratedOn = DateTime.UtcNow
            };

            _db.PerformanceReports.Add(report);
            await _db.SaveChangesAsync();

            // Proceed with file download logic here
            return Ok("Report saved and download initiated.");
        }

        // 🔧 Helper Methods

        private Dictionary<string, int> GetMonthlyAcceptedTrips()
        {
            var tripController = new TripSchedulingController(_db);
            var monthlyAcceptedTrips = new Dictionary<string, int>();

            for (int month = 1; month <= 12; month++)
            {
                int acceptedCount = tripController.GetAcceptedTripCountForMonth(month);
                string monthName = new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM");
                monthlyAcceptedTrips.Add(monthName, acceptedCount);
            }

            return monthlyAcceptedTrips;
        }

        private Dictionary<string, (decimal Quantity, decimal Cost)> GetMonthlyFuelData()
        {
            var monthlyFuelData = new Dictionary<string, (decimal Quantity, decimal Cost)>();

            for (int month = 1; month <= 12; month++)
            {
                var monthFuelRecords = _db.FuelRecords
                    .Where(f => f.Date.Month == month && f.Date.Year == DateTime.Now.Year)
                    .ToList();

                decimal averageQuantity = monthFuelRecords.Any() ? monthFuelRecords.Average(f => f.FuelQuantity) : 0;
                decimal averageCost = monthFuelRecords.Any() ? monthFuelRecords.Average(f => f.Cost) : 0;

                string monthName = new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM");
                monthlyFuelData.Add(monthName, (averageQuantity, averageCost));
            }

            return monthlyFuelData;
        }

        private (int TotalTrips, int AvailableVehicles, int UnavailableVehicles, int ScheduledMaintenance, int CompletedMaintenance) GetFleetStats(Dictionary<string, int> monthlyAcceptedTrips)
        {
            int availableCount = _db.Vehicles.Count(v => v.Status.ToLower() == "available");
            int unavailableCount = _db.Vehicles.Count(v => v.Status.ToLower() == "unavailable");
            int scheduledCount = _db.MaintenanceRecords.Count(m => m.Status.ToLower() == "scheduled");
            int completedCount = _db.MaintenanceRecords.Count(m => m.Status.ToLower() == "completed");
            int totalTrips = monthlyAcceptedTrips.Sum(x => x.Value);

            return (totalTrips, availableCount, unavailableCount, scheduledCount, completedCount);
        }
    }
}
