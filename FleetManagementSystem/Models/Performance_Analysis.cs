using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem.Models
{
    public class Performance_Analysis
    {
        [Key]
        public int PerformanceId { get; set; }

        // Type of report (e.g., "Fuel Efficiency", "Trip Summary")
        [MaxLength(50)]
        public string ReportType { get; set; } = "System Generated";

        // JSON or summary data
        public string Data { get; set; } = string.Empty;

        // Automatically set when the report is created
        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;

        public int TotalTrips { get; set; }
    }
}
