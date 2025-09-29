using FleetManagementSystem.Data;
using FleetManagementSystem.Helpers;
using FleetManagementSystem.Models;
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

        public IActionResult Performance_Analysis( )
        {
           
            var tripController = new TripSchedulingController(_db);

            // 1. Create a dictionary to hold the monthly data
            var monthlyAcceptedTrips = new Dictionary<string, int>();

            // 2. Loop through all 12 months of the year (1 to 12)
            for (int month = 1; month <= 12; month++)
            {
                // Get the count using the new consolidated method
                int acceptedCount = tripController.GetAcceptedTripCountForMonth(month);

                // Add the data to the dictionary
                string monthName = new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM"); // Convert month number to month name
                monthlyAcceptedTrips.Add(monthName, acceptedCount);
            }

            // 3. Create the model for the view (optional, you can just use ViewBag)
            var model = new Performance_Analysis
            {
                TotalTrips = monthlyAcceptedTrips.Sum(x => x.Value) // Calculate total trips from the dictionary
            };

            // 4. Pass the entire dictionary to the ChartGenerator
            var imgBytes = ChartGenerator.GenerateAcceptedTripsChart(monthlyAcceptedTrips);
            ViewBag.ChartImage = Convert.ToBase64String(imgBytes);

            // 5. You can also pass the dictionary to the view if you need to display a table of data
            ViewBag.MonthlyData = monthlyAcceptedTrips;

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

                var fuelChartBytes = FuelChartGenerator.GenerateFuelBarChart(monthlyFuelData);
                ViewBag.FuelChartImage = Convert.ToBase64String(fuelChartBytes);

            }


            var availableCount = _db.Vehicles
     .Where(v => v.Status.ToLower() == "available")
     .Count();

            var unavailableCount = _db.Vehicles
                .Where(v => v.Status.ToLower() == "unavailable")
                .Count();



            ViewBag.AvailableVehicles = availableCount;
            ViewBag.UnavailableVehicles = unavailableCount;

            var scheduledCount = _db.MaintenanceRecords
                   .Where(v => v.Status.ToLower() == "scheduled")
                   .Count();

            var completeedCount = _db.MaintenanceRecords
                  .Where(v => v.Status == "completed")
                  .Count();

            ViewBag.Scheduled = scheduledCount;
            ViewBag.Completed = completeedCount;
            

            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml", model);
        }

    }



}

