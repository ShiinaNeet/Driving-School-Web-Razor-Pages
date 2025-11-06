using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public class InstructorRating
    {
        public int Id { get; set; }

        public int ProfessorId { get; set; }
        public Professor Professor { get; set; } = null!;

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public int? ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public int Rating { get; set; } // 1-5

        [StringLength(1000)]
        public string? Review { get; set; }

        public bool IsAnonymous { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
