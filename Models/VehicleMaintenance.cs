using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum MaintenanceType
    {
        Inspection,
        OilChange,
        TireRotation,
        BrakeService,
        Repair,
        Cleaning,
        Other
    }

    public enum MaintenanceStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public class VehicleMaintenance
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public MaintenanceType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletedDate { get; set; }

        public decimal? Cost { get; set; }

        public decimal? MileageAtService { get; set; }

        [StringLength(200)]
        public string? ServiceProvider { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? PartsReplaced { get; set; }

        public string? PerformedBy { get; set; } // Staff member who performed/arranged

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
