namespace EnrollmentSystem.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? ProfessorId { get; set; }
        public Professor? Professor { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string? Location { get; set; }
        public ScheduleType Type { get; set; }
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

        public int? MaxAttendees { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

    public enum ScheduleType
    {
        Lecture,
        PracticalDriving,
        Theory,
        Exam,
        Assessment,
        Other
    }

    public enum ScheduleStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled,
        Postponed
    }
}
