using FleetManagementSystem.Data;
using FleetManagementSystem.Helpers;
using FleetManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem.Controllers
{
    public class PerformanceAnalysisController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PerformanceAnalysisController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Performance_Analysis()
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

            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml", model);
        }

    }
}
