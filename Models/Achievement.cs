using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public class Achievement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(500)]
        public string? IconUrl { get; set; }

        public int Points { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class StudentAchievement
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; } = null!;

        public DateTime EarnedDate { get; set; } = DateTime.UtcNow;
    }
}
