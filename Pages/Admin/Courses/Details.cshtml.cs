using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Course? Course { get; set; }
        public IList<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
        public IList<Enrollment> RecentEnrollments { get; set; } = new List<Enrollment>();
        public int EnrollmentCount { get; set; }
        public int ActiveEnrollmentCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null)
                return NotFound();

            CourseSubjects = await _context.CourseSubjects
                .Include(cs => cs.Subject)
                .Where(cs => cs.CourseId == id)
                .OrderBy(cs => cs.Order)
                .ToListAsync();

            RecentEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == id)
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(10)
                .ToListAsync();

            EnrollmentCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == id);

            ActiveEnrollmentCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == id && e.Status == EnrollmentStatus.Active);

            TotalRevenue = await _context.Payments
                .Where(p => p.Enrollment.CourseId == id && p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            return Page();
        }
    }
}
