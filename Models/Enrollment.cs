using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public int CourseId { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalFee { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PaidAmount { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Balance { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public ApplicationUser Student { get; set; } = null!;

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum EnrollmentStatus
{
    Pending,
    Approved,
    Active,
    Completed,
    Cancelled,
    Suspended
}
