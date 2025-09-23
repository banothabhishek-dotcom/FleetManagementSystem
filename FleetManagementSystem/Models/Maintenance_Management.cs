using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Models
{

    [Table("MaintenanceRecords")]

    public class Maintenance_Management
    {
        [Key]
        public int MaintenanceId { get; set; }

        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Scheduled date is required.")]
        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = string.Empty;

        // Navigation property
        public Vehicle_Management? Vehicle { get; set; }
    }
}
