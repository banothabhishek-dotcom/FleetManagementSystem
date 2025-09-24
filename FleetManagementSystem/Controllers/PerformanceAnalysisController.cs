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
            int septemberAcceptedCount = tripController.GetSeptemberAcceptedTripCount();
            int octoberAcceptedCount = tripController.GetOctoberAcceptedTripCount();



            var model = new Performance_Analysis
            {
                TotalTrips = septemberAcceptedCount + octoberAcceptedCount // or store separately if needed
            };

            ViewBag.SeptemberTrips = septemberAcceptedCount;
            ViewBag.OctoberTrips = octoberAcceptedCount;

            var imgBytes = ChartGenerator.GenerateAcceptedTripsChart(septemberAcceptedCount, octoberAcceptedCount);
            ViewBag.ChartImage = Convert.ToBase64String(imgBytes);


            return View("~/Views/Admin/PerformanceAnalysis/Performance_Analysis.cshtml", model);
        }

    }
}
