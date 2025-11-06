using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Student.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
        public IList<int> EnrolledCourseIds { get; set; } = new List<int>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public class CourseInfo
        {
            public Course Course { get; set; } = null!;
            public int EnrolledCount { get; set; }
            public int AvailableSlots { get; set; }
            public int SubjectCount { get; set; }
            public bool IsAvailable { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get student's enrolled course IDs
            if (!string.IsNullOrEmpty(userId))
            {
                EnrolledCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == userId)
                    .Select(e => e.CourseId)
                    .ToListAsync();
            }

            var query = _context.Courses
                .Include(c => c.CourseSubjects)
                .ThenInclude(cs => cs.Subject)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchTerm) ||
                    c.Code.Contains(SearchTerm) ||
                    c.Description!.Contains(SearchTerm));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(StatusFilter) && Enum.TryParse<CourseStatus>(StatusFilter, out var status))
            {
                query = query.Where(c => c.Status == status);
            }
            else
            {
                // Default to showing only active courses
                query = query.Where(c => c.Status == CourseStatus.Active);
            }

            var courses = await query
                .OrderBy(c => c.Name)
                .ToListAsync();

            Courses = new List<CourseInfo>();

            foreach (var course in courses)
            {
                var enrolledCount = await _context.Enrollments
                    .CountAsync(e => e.CourseId == course.Id &&
                                    (e.Status == EnrollmentStatus.Active ||
                                     e.Status == EnrollmentStatus.Approved ||
                                     e.Status == EnrollmentStatus.Pending));

                var availableSlots = course.MaxStudents.HasValue
                    ? course.MaxStudents.Value - enrolledCount
                    : int.MaxValue;

                Courses.Add(new CourseInfo
                {
                    Course = course,
                    EnrolledCount = enrolledCount,
                    AvailableSlots = availableSlots,
                    SubjectCount = course.CourseSubjects.Count,
                    IsAvailable = course.Status == CourseStatus.Active &&
                                 (course.MaxStudents == null || availableSlots > 0)
                });
            }
        }
    }
}
