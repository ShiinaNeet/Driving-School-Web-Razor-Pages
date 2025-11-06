using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityType { get; set; }

        public string? EntityId { get; set; }

        public string? OldValues { get; set; } // JSON
        public string? NewValues { get; set; } // JSON

        [StringLength(500)]
        public string? IpAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
