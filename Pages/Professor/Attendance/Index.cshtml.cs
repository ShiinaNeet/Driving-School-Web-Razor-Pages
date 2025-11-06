using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Attendance
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

        public IList<Schedule> TodaySchedules { get; set; } = new List<Schedule>();
        public IList<Schedule> UpcomingSchedules { get; set; } = new List<Schedule>();
        public IList<Schedule> RecentSchedules { get; set; } = new List<Schedule>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // Get today's schedules
            TodaySchedules = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => s.ProfessorId == professor.Id &&
                           s.StartTime.Date == today &&
                           s.Status != ScheduleStatus.Cancelled)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Get upcoming schedules (next 7 days)
            UpcomingSchedules = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => s.ProfessorId == professor.Id &&
                           s.StartTime.Date > today &&
                           s.StartTime.Date <= today.AddDays(7) &&
                           s.Status != ScheduleStatus.Cancelled)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Get recent schedules (last 7 days)
            RecentSchedules = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .Where(s => s.ProfessorId == professor.Id &&
                           s.StartTime.Date < today &&
                           s.StartTime.Date >= today.AddDays(-7))
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return Page();
        }

        public int GetAttendanceCount(Schedule schedule)
        {
            return schedule.Attendances?.Count ?? 0;
        }

        public int GetPresentCount(Schedule schedule)
        {
            return schedule.Attendances?.Count(a => a.Status == AttendanceStatus.Present) ?? 0;
        }

        public string GetAttendanceStatus(Schedule schedule)
        {
            var attendanceCount = GetAttendanceCount(schedule);
            if (attendanceCount == 0)
                return "Not Taken";
            return "Taken";
        }
    }
}
