using Microsoft.AspNetCore.Identity;

namespace EnrollmentSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? ProfilePhoto { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Professor? Professor { get; set; }
    public ICollection<Guardian> GuardiansAsUser { get; set; } = new List<Guardian>();
    public ICollection<Guardian> GuardiansAsStudent { get; set; } = new List<Guardian>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<BookBorrowing> BookBorrowings { get; set; } = new List<BookBorrowing>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
    public ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum UserStatus
{
    Active,
    Inactive,
    Suspended
}
