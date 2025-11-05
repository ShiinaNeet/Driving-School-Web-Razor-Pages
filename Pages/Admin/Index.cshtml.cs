using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
    public int ActiveEnrollments { get; set; }
    public int TotalProfessors { get; set; }
    public List<Enrollment> RecentEnrollments { get; set; } = new();
    public List<Payment> RecentPayments { get; set; } = new();

    public async Task OnGetAsync()
    {
        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
        if (studentRole != null)
        {
            TotalStudents = await _context.UserRoles.CountAsync(ur => ur.RoleId == studentRole.Id);
        }

        TotalCourses = await _context.Courses.CountAsync(c => c.Status == CourseStatus.Active);
        ActiveEnrollments = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);
        TotalProfessors = await _context.Professors.CountAsync();

        RecentEnrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrollmentDate)
            .Take(5)
            .ToListAsync();

        RecentPayments = await _context.Payments
            .Include(p => p.Student)
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .ToListAsync();
    }
}
