using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int DurationWeeks { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalFee { get; set; } = 0;

    public int MaxStudents { get; set; } = 30;

    public CourseStatus Status { get; set; } = CourseStatus.Active;

    public string? Prerequisites { get; set; }

    public string? CertificateTemplate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public enum CourseStatus
{
    Active,
    Inactive,
    Archived
}
