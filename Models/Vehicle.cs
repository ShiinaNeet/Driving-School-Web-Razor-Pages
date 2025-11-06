using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum VehicleStatus
    {
        Available,
        InUse,
        Maintenance,
        OutOfService,
        Retired
    }

    public enum VehicleType
    {
        Sedan,
        SUV,
        Truck,
        Van,
        Motorcycle,
        Other
    }

    public enum TransmissionType
    {
        Automatic,
        Manual
    }

    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VIN { get; set; } = string.Empty; // Vehicle Identification Number

        public VehicleType Type { get; set; }

        public TransmissionType Transmission { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        public int Capacity { get; set; } // Passenger capacity

        public decimal CurrentMileage { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RegistrationExpiry { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InsuranceExpiry { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InspectionExpiry { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastServiceDate { get; set; }

        public decimal? LastServiceMileage { get; set; }

        public int? NextServiceMileage { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NextServiceDate { get; set; }

        public decimal? FuelLevel { get; set; } // Percentage 0-100

        public int? LocationId { get; set; } // For multi-location support
        public Location? Location { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<VehicleMaintenance> MaintenanceRecords { get; set; } = new List<VehicleMaintenance>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
