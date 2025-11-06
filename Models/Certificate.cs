using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum CertificateStatus
    {
        Draft,
        Issued,
        Revoked,
        Expired
    }

    public enum CertificateType
    {
        CourseCompletion,
        DriversEducation,
        DefensiveDriving,
        AdvancedDriving,
        Custom
    }

    public class Certificate
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CertificateNumber { get; set; } = string.Empty;

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public int? EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public CertificateType Type { get; set; }

        public CertificateStatus Status { get; set; } = CertificateStatus.Draft;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(200)]
        public string? IssuedBy { get; set; }

        [StringLength(500)]
        public string? VerificationCode { get; set; } // QR code data

        [StringLength(500)]
        public string? FilePath { get; set; } // Path to generated PDF

        public string? TemplateId { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
