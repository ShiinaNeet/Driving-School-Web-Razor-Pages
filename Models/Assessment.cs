namespace EnrollmentSystem.Models
{
    public class Assessment
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public AssessmentType Type { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; } = 100;

        public bool Passed { get; set; }
        public DateTime AssessmentDate { get; set; }

        public string? AssessedBy { get; set; }
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum AssessmentType
    {
        WrittenTest,
        PracticalDriving,
        TheoryExam,
        FinalExam,
        Assignment,
        Quiz,
        Other
    }
}
