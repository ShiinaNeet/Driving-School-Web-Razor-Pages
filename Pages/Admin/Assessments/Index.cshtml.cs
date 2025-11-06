using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Assessments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AssessmentRecord> Assessments { get; set; } = new List<AssessmentRecord>();
        public SelectList CourseList { get; set; } = null!;
        public SelectList SubjectList { get; set; } = null!;
        public SelectList StudentList { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int? FilterCourseId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterSubjectId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterType { get; set; }

        // Statistics
        public int TotalAssessments { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public decimal AverageScore { get; set; }
        public decimal PassRate { get; set; }

        public class AssessmentRecord
        {
            public Assessment Assessment { get; set; } = null!;
            public Enrollment Enrollment { get; set; } = null!;
            public ApplicationUser Student { get; set; } = null!;
        }

        public async Task OnGetAsync()
        {
            // Load filter options
            var courses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            CourseList = new SelectList(courses, "Id", "Name");

            var subjects = await _context.Subjects.OrderBy(s => s.Code).ToListAsync();
            SubjectList = new SelectList(
                subjects.Select(s => new
                {
                    s.Id,
                    Name = $"{s.Code} - {s.Name}"
                }),
                "Id",
                "Name"
            );

            var students = await _context.Students
                .OrderBy(s => s.FirstName)
                .ToListAsync();
            StudentList = new SelectList(
                students.Select(s => new
                {
                    s.Id,
                    Name = $"{s.FirstName} {s.LastName}"
                }),
                "Id",
                "Name"
            );

            // Query assessments with filters
            var query = _context.Assessments
                .Include(a => a.Subject)
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Student)
                .Include(a => a.Enrollment)
                .ThenInclude(e => e.Course)
                .AsQueryable();

            // Apply filters
            if (FilterCourseId.HasValue)
                query = query.Where(a => a.Enrollment.CourseId == FilterCourseId.Value);

            if (FilterSubjectId.HasValue)
                query = query.Where(a => a.SubjectId == FilterSubjectId.Value);

            if (!string.IsNullOrEmpty(FilterStudentId))
                query = query.Where(a => a.Enrollment.StudentId == FilterStudentId);

            if (!string.IsNullOrEmpty(FilterType) && Enum.TryParse<AssessmentType>(FilterType, out var type))
                query = query.Where(a => a.Type == type);

            var assessments = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            Assessments = assessments.Select(a => new AssessmentRecord
            {
                Assessment = a,
                Enrollment = a.Enrollment,
                Student = a.Enrollment.Student
            }).ToList();

            // Calculate statistics
            TotalAssessments = assessments.Count;
            PassedCount = assessments.Count(a => a.Passed);
            FailedCount = assessments.Count(a => !a.Passed);
            AverageScore = assessments.Any()
                ? assessments.Average(a => (a.Score / a.MaxScore) * 100)
                : 0;
            PassRate = TotalAssessments > 0
                ? ((decimal)PassedCount / TotalAssessments) * 100
                : 0;
        }
    }
}
