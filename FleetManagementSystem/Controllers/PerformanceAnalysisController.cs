using FleetManagementSystem.Data;
using FleetManagementSystem.Helpers;
using FleetManagementSystem.Models;
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
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Customer");
        }

        public IActionResult Performance_Analysis()
        {
            ViewBag.HideFooter = true;
            // Retrieve monthly accepted trips and fuel data
            var monthlyAcceptedTrips = GetMonthlyAcceptedTrips();
            var monthlyFuelData = GetMonthlyFuelData();
            // Generate chart images as byte arrays
            var acceptedTripsChartBytes = ChartGenerator.GenerateAcceptedTripsChart(monthlyAcceptedTrips);
            var fuelChartBytes = FuelChartGenerator.GenerateFuelBarChart(monthlyFuelData);
            // Calculate fleet statistics based on trip data
            var stats = GetFleetStats(monthlyAcceptedTrips);

            // Create model with total trips
            var model = new Performance_Analysis
            {
                TotalTrips = stats.TotalTrips
            };

            // Pass chart images and stats to the view using ViewBag
            ViewBag.ChartImage = Convert.ToBase64String(acceptedTripsChartBytes);
            ViewBag.FuelChartImage = Convert.ToBase64String(fuelChartBytes);
            ViewBag.MonthlyData = monthlyAcceptedTrips;
            ViewBag.AvailableVehicles = stats.AvailableVehicles;
            ViewBag.UnavailableVehicles = stats.UnavailableVehicles;
            ViewBag.Scheduled = stats.ScheduledMaintenance;
            ViewBag.Completed = stats.CompletedMaintenance;

            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml", model);
        }

        // HTTP GET action to generate and download a performance PDF report
        [HttpGet]
        public async Task<IActionResult> DownloadPerformancePdf()
        {
            // 1. Get data
            var monthlyAcceptedTrips = GetMonthlyAcceptedTrips();
            var monthlyFuelData = GetMonthlyFuelData();
            var acceptedTripsChartBytes = ChartGenerator.GenerateAcceptedTripsChart(monthlyAcceptedTrips);
            var fuelChartBytes = FuelChartGenerator.GenerateFuelBarChart(monthlyFuelData);
            var stats = GetFleetStats(monthlyAcceptedTrips);

            // 2. Save report details in DB
            var report = new Performance_Analysis
            {
                ReportType = "pdf",
                Data = $"Total trips: {stats.TotalTrips}\n" +
                       $"Scheduled maintenance: {stats.ScheduledMaintenance}\n" +
                       $"Completed maintenance: {stats.CompletedMaintenance}\n" +
                       $"Available vehicles: {stats.AvailableVehicles}\n" +
                       $"Unavailable vehicles: {stats.UnavailableVehicles}",
                GeneratedOn = DateTime.UtcNow
            };

            _db.PerformanceReports.Add(report);
            await _db.SaveChangesAsync();

            // 3. Generate PDF
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Title
                doc.Add(new Paragraph("Fleet Performance Summary"));
                doc.Add(new Paragraph($"Generated On: {DateTime.Now}"));
                doc.Add(new Paragraph($"Total Trips: {stats.TotalTrips}"));
                doc.Add(new Paragraph($"Maintenance - Scheduled: {stats.ScheduledMaintenance}, Completed: {stats.CompletedMaintenance}"));
                doc.Add(new Paragraph($"Vehicles - Available: {stats.AvailableVehicles}, Unavailable: {stats.UnavailableVehicles}"));
                doc.Add(new Paragraph(" "));

                // Charts
                iTextSharp.text.Image img1 = iTextSharp.text.Image.GetInstance(acceptedTripsChartBytes);
                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(fuelChartBytes);
                img1.ScaleToFit(500f, 300f);
                img2.ScaleToFit(500f, 300f);

                doc.Add(new Paragraph("Accepted Trips Overview"));
                doc.Add(img1);
                doc.Add(new Paragraph("Fuel Consumption Overview"));
                doc.Add(img2);

                doc.Close();

                // 4. Return PDF
                return File(ms.ToArray(), "application/pdf", "PerformanceReport.pdf");
            }
        }

        // 🔧 Helper Methods
        private Dictionary<string, int> GetMonthlyAcceptedTrips()
        {
            var tripController = new TripSchedulingController(_db);
            var monthlyAcceptedTrips = new Dictionary<string, int>();

            for (int month = 1; month <= 12; month++)
            {
                // Get accepted trip count for the month
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
                // Filter fuel records by month and current year
                var monthFuelRecords = _db.FuelRecords
                    .Where(f => f.Date.Month == month && f.Date.Year == DateTime.Now.Year)
                    .ToList();

                // Calculate averages if records exist
                decimal averageQuantity = monthFuelRecords.Any() ? monthFuelRecords.Average(f => f.FuelQuantity) : 0;
                decimal averageCost = monthFuelRecords.Any() ? monthFuelRecords.Average(f => f.Cost) : 0;
                // Convert month number to name
                string monthName = new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM");

                // Store in dictionary
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