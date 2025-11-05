using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Assessments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<StudentGrades> StudentGradesList { get; set; } = new List<StudentGrades>();
        public IList<Subject> ProfessorSubjects { get; set; } = new List<Subject>();
        public SelectList CourseList { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int? SelectedCourseId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedSubjectId { get; set; }

        public class StudentGrades
        {
            public string StudentId { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int EnrollmentId { get; set; }
            public string CourseName { get; set; } = string.Empty;
            public IList<Assessment> Assessments { get; set; } = new List<Assessment>();
            public decimal AverageScore { get; set; }
            public int TotalAssessments { get; set; }
            public int PassedAssessments { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            // Get professor's subjects
            ProfessorSubjects = await _context.Subjects
                .Where(s => s.ProfessorId == professor.Id)
                .OrderBy(s => s.Code)
                .ToListAsync();

            // Get courses that have professor's subjects
            var subjectIds = ProfessorSubjects.Select(s => s.Id).ToList();
            var courses = await _context.Courses
                .Where(c => c.CourseSubjects.Any(cs => subjectIds.Contains(cs.SubjectId)))
                .OrderBy(c => c.Name)
                .ToListAsync();

            CourseList = new SelectList(courses, "Id", "Name");

            // Get enrollments based on filters
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Assessments)
                .ThenInclude(a => a.Subject)
                .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved);

            if (SelectedCourseId.HasValue)
            {
                query = query.Where(e => e.CourseId == SelectedCourseId.Value);
            }
            else
            {
                // Show all students in courses with professor's subjects
                query = query.Where(e => e.Course.CourseSubjects.Any(cs => subjectIds.Contains(cs.SubjectId)));
            }

            var enrollments = await query.ToListAsync();

            StudentGradesList = enrollments.Select(e =>
            {
                var assessments = e.Assessments.ToList();
                if (SelectedSubjectId.HasValue)
                {
                    assessments = assessments.Where(a => a.SubjectId == SelectedSubjectId.Value).ToList();
                }

                return new StudentGrades
                {
                    StudentId = e.StudentId,
                    StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
                    Email = e.Student.Email!,
                    EnrollmentId = e.Id,
                    CourseName = e.Course.Name,
                    Assessments = assessments,
                    TotalAssessments = assessments.Count,
                    PassedAssessments = assessments.Count(a => a.Passed),
                    AverageScore = assessments.Any()
                        ? assessments.Average(a => (a.Score / a.MaxScore) * 100)
                        : 0
                };
            }).OrderBy(s => s.StudentName).ToList();

            return Page();
        }
    }
}
