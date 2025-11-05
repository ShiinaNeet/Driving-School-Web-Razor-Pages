using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class CourseSubject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    public int Order { get; set; } = 0;

    public bool IsRequired { get; set; } = true;

    // Navigation properties
    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [ForeignKey("SubjectId")]
    public Subject Subject { get; set; } = null!;
}
