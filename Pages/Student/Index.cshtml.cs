using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Student;

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
    public List<Enrollment> MyEnrollments { get; set; } = new();
    public List<Payment> MyPayments { get; set; } = new();
    public List<Schedule> UpcomingClasses { get; set; } = new();
    public List<Assessment> RecentGrades { get; set; } = new();
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }
    public int ActiveEnrollmentsCount { get; set; }

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User) ?? new ApplicationUser();

        MyEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Assessments)
                .ThenInclude(a => a.Subject)
            .Where(e => e.StudentId == CurrentUser.Id)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();

        MyPayments = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
            .Where(p => p.StudentId == CurrentUser.Id)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        // Get upcoming classes
        var enrolledCourseIds = MyEnrollments.Select(e => e.CourseId).ToList();
        UpcomingClasses = await _context.Schedules
            .Include(s => s.Course)
            .Include(s => s.Subject)
            .Where(s => enrolledCourseIds.Contains(s.CourseId) &&
                       s.StartTime >= DateTime.UtcNow &&
                       s.Status == ScheduleStatus.Scheduled)
            .OrderBy(s => s.StartTime)
            .Take(5)
            .ToListAsync();

        // Get recent grades
        var enrollmentIds = MyEnrollments.Select(e => e.Id).ToList();
        RecentGrades = await _context.Assessments
            .Include(a => a.Subject)
            .Include(a => a.Enrollment)
                .ThenInclude(e => e.Course)
            .Where(a => enrollmentIds.Contains(a.EnrollmentId))
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToListAsync();

        TotalPaid = MyPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        TotalBalance = MyEnrollments.Sum(e => e.Balance);
        ActiveEnrollmentsCount = MyEnrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved);
    }
}
