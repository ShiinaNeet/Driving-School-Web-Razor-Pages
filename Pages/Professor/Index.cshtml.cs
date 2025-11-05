using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor
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

        public string ProfessorName { get; set; } = string.Empty;
        public int SubjectCount { get; set; }
        public int UpcomingScheduleCount { get; set; }
        public int PendingAssessmentCount { get; set; }
        public int TotalStudents { get; set; }
        public IList<Subject> Subjects { get; set; } = new List<Subject>();
        public IList<Schedule> UpcomingSchedules { get; set; } = new List<Schedule>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                ProfessorName = $"{user.FirstName} {user.LastName}";
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor != null)
            {
                Subjects = await _context.Subjects
                    .Where(s => s.ProfessorId == professor.Id)
                    .OrderBy(s => s.Code)
                    .ToListAsync();

                SubjectCount = Subjects.Count;

                UpcomingSchedules = await _context.Schedules
                    .Include(s => s.Course)
                    .Include(s => s.Subject)
                    .Where(s => s.ProfessorId == professor.Id &&
                               s.StartTime >= DateTime.UtcNow &&
                               s.Status == ScheduleStatus.Scheduled)
                    .OrderBy(s => s.StartTime)
                    .Take(5)
                    .ToListAsync();

                UpcomingScheduleCount = await _context.Schedules
                    .CountAsync(s => s.ProfessorId == professor.Id &&
                                    s.StartTime >= DateTime.UtcNow &&
                                    s.Status == ScheduleStatus.Scheduled);

                // Count unique students enrolled in courses with professor's subjects
                var subjectIds = Subjects.Select(s => s.Id).ToList();
                TotalStudents = await _context.Enrollments
                    .Where(e => e.Course.CourseSubjects.Any(cs => subjectIds.Contains(cs.SubjectId)) &&
                               (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved))
                    .Select(e => e.StudentId)
                    .Distinct()
                    .CountAsync();

                PendingAssessmentCount = 0; // Placeholder - implement based on business logic
            }
        }
    }
}
