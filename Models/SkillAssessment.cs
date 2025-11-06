using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum SkillLevel
    {
        Beginner = 1,
        Novice = 2,
        Intermediate = 3,
        Advanced = 4,
        Expert = 5
    }

    public class DrivingSkill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Category { get; set; } = string.Empty; // Parking, Highway, Maneuvering, etc.

        public bool IsActive { get; set; } = true;
    }

    public class SkillAssessment
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public int SkillId { get; set; }
        public DrivingSkill Skill { get; set; } = null!;

        public int? ProfessorId { get; set; }
        public Professor? Professor { get; set; }

        public SkillLevel Level { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime AssessedDate { get; set; }
    }
}
