using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Models
{
    [Table("Fuel_Management")]
    public class Fuel_Management
    {
        [Key]
        public int FuelId { get; set; }

        [Required]
        [ForeignKey("Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Fuel quantity must be greater than 0.")]
        public decimal FuelQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0.")]
        public decimal Cost { get; set; }

        // Navigation Property
        public Vehicle_Management? Vehicle { get; set; }
    }
}
