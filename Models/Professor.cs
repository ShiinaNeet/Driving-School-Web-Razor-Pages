using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class Professor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string EmployeeId { get; set; } = string.Empty;

    public string? Specialization { get; set; }

    public string? Qualification { get; set; }

    public string? Bio { get; set; }

    public DateTime? HireDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? Salary { get; set; }

    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.FullTime;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}

public enum EmploymentStatus
{
    FullTime,
    PartTime,
    Contract
}
