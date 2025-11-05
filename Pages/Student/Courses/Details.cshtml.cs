using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Student.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Course Course { get; set; } = null!;
        public IList<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
        public IList<Schedule> UpcomingSchedules { get; set; } = new List<Schedule>();
        public int EnrolledCount { get; set; }
        public int AvailableSlots { get; set; }
        public bool IsEnrolled { get; set; }
        public Enrollment? UserEnrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Course == null)
                return NotFound();

            // Get course subjects
            CourseSubjects = await _context.CourseSubjects
                .Include(cs => cs.Subject)
                    .ThenInclude(s => s.Professor)
                        .ThenInclude(p => p!.User)
                .Where(cs => cs.CourseId == id)
                .OrderBy(cs => cs.Order)
                .ToListAsync();

            // Get upcoming schedules
            UpcomingSchedules = await _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Professor)
                    .ThenInclude(p => p!.User)
                .Where(s => s.CourseId == id &&
                           s.StartTime >= DateTime.UtcNow &&
                           s.Status == ScheduleStatus.Scheduled)
                .OrderBy(s => s.StartTime)
                .Take(5)
                .ToListAsync();

            // Get enrollment statistics
            EnrolledCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == id &&
                                (e.Status == EnrollmentStatus.Active ||
                                 e.Status == EnrollmentStatus.Approved ||
                                 e.Status == EnrollmentStatus.Pending));

            AvailableSlots = Course.MaxStudents.HasValue
                ? Course.MaxStudents.Value - EnrolledCount
                : int.MaxValue;

            // Check if current user is enrolled
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                UserEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == id);
                IsEnrolled = UserEnrollment != null;
            }

            return Page();
        }
    }
}
