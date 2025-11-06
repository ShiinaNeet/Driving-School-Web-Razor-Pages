using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum DocumentType
    {
        DriversPermit,
        ParentConsent,
        MedicalCertificate,
        Insurance,
        IdentificationCard,
        Other
    }

    public class StudentDocument
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public DocumentType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsVerified { get; set; } = false;

        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
}
