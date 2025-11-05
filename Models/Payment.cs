using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EnrollmentId { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    public string? Remarks { get; set; }

    [Required]
    public string RecordedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("EnrollmentId")]
    public Enrollment Enrollment { get; set; } = null!;

    [ForeignKey("StudentId")]
    public ApplicationUser Student { get; set; } = null!;
}

public enum PaymentMethod
{
    Cash,
    Check,
    BankTransfer,
    CreditCard,
    DebitCard,
    Other
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
