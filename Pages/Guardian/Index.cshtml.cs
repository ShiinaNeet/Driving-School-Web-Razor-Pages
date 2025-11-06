using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Guardian;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public ApplicationUser CurrentUser { get; set; } = null!;
    public List<StudentProgressInfo> StudentsProgress { get; set; } = new();

    public class StudentProgressInfo
    {
        public EnrollmentSystem.Models.Guardian GuardianRecord { get; set; } = null!;
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
        public List<Schedule> UpcomingClasses { get; set; } = new();
        public List<Assessment> RecentGrades { get; set; } = new();
        public decimal AverageGrade { get; set; }
        public int AttendancePercentage { get; set; }
    }

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User) ?? new ApplicationUser();

        var guardianRecords = await _context.Guardians
            .Include(g => g.Student)
            .Where(g => g.UserId == CurrentUser.Id)
            .ToListAsync();

        StudentsProgress = new List<StudentProgressInfo>();

        foreach (var guardian in guardianRecords)
        {
            var studentId = guardian.StudentId;

            // Get enrollments
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Assessments)
                .ThenInclude(a => a.Subject)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            // Get payments
            var payments = await _context.Payments
                .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            // Get upcoming classes
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
            var upcomingClasses = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Where(s => enrolledCourseIds.Contains(s.CourseId) &&
                           s.StartTime >= DateTime.UtcNow &&
                           s.Status == ScheduleStatus.Scheduled)
                .OrderBy(s => s.StartTime)
                .Take(5)
                .ToListAsync();

            // Get recent grades
            var enrollmentIds = enrollments.Select(e => e.Id).ToList();
            var recentGrades = await _context.Assessments
                .Include(a => a.Subject)
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Course)
                .Where(a => enrollmentIds.Contains(a.EnrollmentId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Calculate average grade
            var allGrades = enrollments.SelectMany(e => e.Assessments).ToList();
            var averageGrade = allGrades.Any()
                ? allGrades.Average(a => (a.Score / a.MaxScore) * 100)
                : 0;

            // Calculate attendance
            var totalClasses = await _context.Schedules
                .CountAsync(s => enrolledCourseIds.Contains(s.CourseId) &&
                                s.StartTime < DateTime.UtcNow);

            var attendedClasses = await _context.Attendances
                .CountAsync(a => a.StudentId == studentId &&
                                a.Status == AttendanceStatus.Present);

            var attendancePercentage = totalClasses > 0
                ? (int)((double)attendedClasses / totalClasses * 100)
                : 0;

            StudentsProgress.Add(new StudentProgressInfo
            {
                GuardianRecord = guardian,
                Enrollments = enrollments,
                Payments = payments,
                UpcomingClasses = upcomingClasses,
                RecentGrades = recentGrades,
                AverageGrade = averageGrade,
                AttendancePercentage = attendancePercentage
            });
        }
    }
}
