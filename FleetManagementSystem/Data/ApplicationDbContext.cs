using FleetManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Vehicle_Management> Vehicles { get; set; }
        public DbSet<Fuel_Management> FuelRecords { get; set; }
        public DbSet<Maintenance_Management> MaintenanceRecords { get; set; }
        public DbSet<Trip_Scheduling> Trips { get; set; }
        public DbSet<Performance_Analysis> PerformanceReports { get; set; }
        public DbSet<User_Details> UserDetails { get; set; }

        internal async Task<string> FindAsync(string? userId)
        {
            throw new NotImplementedException();
        }
    }
}
