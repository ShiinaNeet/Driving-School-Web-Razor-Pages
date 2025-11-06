using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Schedules
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

        public IList<Schedule> Schedules { get; set; } = new List<Schedule>();
        public IList<Schedule> UpcomingSchedules { get; set; } = new List<Schedule>();
        public IList<Schedule> PastSchedules { get; set; } = new List<Schedule>();
        public int TotalSchedules { get; set; }
        public int CompletedCount { get; set; }
        public int UpcomingCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterType { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            var query = _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Where(s => s.ProfessorId == professor.Id);

            // Apply filters
            if (!string.IsNullOrEmpty(FilterType))
            {
                if (Enum.TryParse<ScheduleType>(FilterType, out var scheduleType))
                {
                    query = query.Where(s => s.Type == scheduleType);
                }
            }

            if (FilterDate.HasValue)
            {
                var filterDate = FilterDate.Value.Date;
                query = query.Where(s => s.StartTime.Date == filterDate);
            }

            Schedules = await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Get statistics
            var allSchedules = await _context.Schedules
                .Where(s => s.ProfessorId == professor.Id)
                .ToListAsync();

            TotalSchedules = allSchedules.Count;
            CompletedCount = allSchedules.Count(s => s.Status == ScheduleStatus.Completed);
            UpcomingCount = allSchedules.Count(s => s.Status == ScheduleStatus.Scheduled && s.StartTime >= DateTime.UtcNow);

            UpcomingSchedules = allSchedules
                .Where(s => s.StartTime >= DateTime.UtcNow && s.Status == ScheduleStatus.Scheduled)
                .OrderBy(s => s.StartTime)
                .Take(10)
                .ToList();

            PastSchedules = allSchedules
                .Where(s => s.StartTime < DateTime.UtcNow || s.Status == ScheduleStatus.Completed)
                .OrderByDescending(s => s.StartTime)
                .Take(10)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int scheduleId, string status)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule == null)
                return NotFound();

            if (Enum.TryParse<ScheduleStatus>(status, out var scheduleStatus))
            {
                schedule.Status = scheduleStatus;
                schedule.UpdatedAt = DateTime.UtcNow;

                if (scheduleStatus == ScheduleStatus.Completed)
                {
                    schedule.EndTime = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = "Schedule status updated successfully.";
            }

            return RedirectToPage();
        }
    }
}
