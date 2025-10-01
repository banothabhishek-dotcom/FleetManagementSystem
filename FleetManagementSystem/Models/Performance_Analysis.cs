using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Models
{
    [Table("Performance")]
    public class Performance_Analysis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PerformanceId { get; set; }

        [MaxLength(50)]
        public string ReportType { get; set; } = "System Generated";

        public string Data { get; set; } = string.Empty;

        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int TotalTrips { get; set; }
    }
}