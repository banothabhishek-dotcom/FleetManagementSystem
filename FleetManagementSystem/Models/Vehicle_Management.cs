using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem.Models
{
    public class Vehicle_Management
    {
        [Key]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Registration number is required.")]
        [MaxLength(50, ErrorMessage = "Registration number cannot exceed 50 characters.")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last serviced date is required.")]
        [DataType(DataType.Date)]
        public DateTime LastServicedDate { get; set; }

        [Required(ErrorMessage = "Driver name is required.")]
        [MaxLength(50, ErrorMessage = "Driver name cannot exceed 50 characters.")]
        public string DriverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Driver phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Driver phone must be a 10-digit number.")]
        public int DriverPhone { get; set; }

        // Navigation Properties
        public ICollection<Trip_Scheduling> Trips { get; set; } = new List<Trip_Scheduling>();
        public ICollection<Fuel_Management> FuelRecords { get; set; } = new List<Fuel_Management>();
        public ICollection<Performance_Analysis> PerformanceReports { get; set; } = new List<Performance_Analysis>();
        public ICollection<Maintanence_Management> MaintenanceRecords { get; set; } = new List<Maintanence_Management>();
    }
}
