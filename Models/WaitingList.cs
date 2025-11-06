using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public enum WaitingListStatus
    {
        Active,
        Enrolled,
        Cancelled,
        Expired
    }

    public class WaitingList
    {
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int Priority { get; set; } = 0;

        public WaitingListStatus Status { get; set; } = WaitingListStatus.Active;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime? EnrolledDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
