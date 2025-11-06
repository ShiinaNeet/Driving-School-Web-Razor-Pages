using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public class TheoryTest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int PassingScore { get; set; } = 70;
        public int TimeLimit { get; set; } // Minutes
        public int MaxAttempts { get; set; } = 3;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TheoryQuestion> Questions { get; set; } = new List<TheoryQuestion>();
        public ICollection<TheoryTestAttempt> Attempts { get; set; } = new List<TheoryTestAttempt>();
    }

    public class TheoryQuestion
    {
        public int Id { get; set; }

        [Required]
        public int TestId { get; set; }
        public TheoryTest Test { get; set; } = null!;

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public string OptionA { get; set; } = string.Empty;
        [Required]
        public string OptionB { get; set; } = string.Empty;
        [Required]
        public string OptionC { get; set; } = string.Empty;
        [Required]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, or D

        public string? Explanation { get; set; }
        public int Points { get; set; } = 1;
        public int OrderIndex { get; set; }
    }

    public class TheoryTestAttempt
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public TheoryTest Test { get; set; } = null!;

        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = null!;

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public bool Passed { get; set; }

        public string? Answers { get; set; } // JSON of answers
    }
}
