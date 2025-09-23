using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Models
{
    [Table("Trip_Scheduling")]
    public class Trip_Scheduling
    {
        [Key]
        public int TripId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pickup point is required.")]
        [MaxLength(50, ErrorMessage = "Pickup point cannot exceed 50 characters.")]
        public string PickupPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Drop point is required.")]
        [MaxLength(50, ErrorMessage = "Drop point cannot exceed 50 characters.")]
        public string DropPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle type is required.")]
        [MaxLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters.")]
        public string VehicleType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Booking time is required.")]
        [DataType(DataType.DateTime)]
        public DateTime BookingTime { get; set; }

        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }

        [MaxLength(50, ErrorMessage = "Driver name cannot exceed 50 characters.")]
        public string? AssignedDriver { get; set; } = string.Empty;

        // Navigation Property
        public Vehicle_Management? Vehicle { get; set; }
    }
}
