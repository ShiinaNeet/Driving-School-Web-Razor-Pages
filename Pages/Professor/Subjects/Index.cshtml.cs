using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Subjects
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

        public IList<SubjectInfo> Subjects { get; set; } = new List<SubjectInfo>();

        public class SubjectInfo
        {
            public Subject Subject { get; set; } = null!;
            public int EnrolledStudents { get; set; }
            public int MaterialsCount { get; set; }
            public int UpcomingClasses { get; set; }
            public IList<Course> Courses { get; set; } = new List<Course>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            var professorSubjects = await _context.Subjects
                .Where(s => s.ProfessorId == professor.Id)
                .OrderBy(s => s.Code)
                .ToListAsync();

            Subjects = new List<SubjectInfo>();

            foreach (var subject in professorSubjects)
            {
                // Get courses using this subject
                var courses = await _context.CourseSubjects
                    .Include(cs => cs.Course)
                    .Where(cs => cs.SubjectId == subject.Id)
                    .Select(cs => cs.Course)
                    .ToListAsync();

                // Get enrolled students count (unique students in courses with this subject)
                var courseIds = courses.Select(c => c.Id).ToList();
                var enrolledStudents = await _context.Enrollments
                    .Where(e => courseIds.Contains(e.CourseId) &&
                               (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved))
                    .Select(e => e.StudentId)
                    .Distinct()
                    .CountAsync();

                // Get materials count
                var materialsCount = await _context.SubjectMaterials
                    .CountAsync(sm => sm.SubjectId == subject.Id);

                // Get upcoming classes count
                var upcomingClasses = await _context.Schedules
                    .CountAsync(s => s.SubjectId == subject.Id &&
                                    s.ProfessorId == professor.Id &&
                                    s.StartTime >= DateTime.UtcNow &&
                                    s.Status == ScheduleStatus.Scheduled);

                Subjects.Add(new SubjectInfo
                {
                    Subject = subject,
                    EnrolledStudents = enrolledStudents,
                    MaterialsCount = materialsCount,
                    UpcomingClasses = upcomingClasses,
                    Courses = courses
                });
            }

            return Page();
        }
    }
}
