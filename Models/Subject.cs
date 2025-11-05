using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class Subject
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

    public int Credits { get; set; } = 3;

    public int? ProfessorId { get; set; }

    public SubjectStatus Status { get; set; } = SubjectStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("ProfessorId")]
    public Professor? Professor { get; set; }

    public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
}

public enum SubjectStatus
{
    Active,
    Inactive
}
