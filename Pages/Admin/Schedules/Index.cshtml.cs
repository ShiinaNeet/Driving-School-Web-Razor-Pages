using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Schedules
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Schedule> Schedules { get; set; } = new List<Schedule>();
        public SelectList CourseList { get; set; } = null!;
        public SelectList ProfessorList { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int? FilterCourseId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterProfessorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDate { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Professor)
                    .ThenInclude(p => p!.User)
                .AsQueryable();

            // Apply filters
            if (FilterCourseId.HasValue)
                query = query.Where(s => s.CourseId == FilterCourseId.Value);

            if (FilterProfessorId.HasValue)
                query = query.Where(s => s.ProfessorId == FilterProfessorId.Value);

            if (!string.IsNullOrEmpty(FilterStatus) && Enum.TryParse<ScheduleStatus>(FilterStatus, out var status))
                query = query.Where(s => s.Status == status);

            if (FilterDate.HasValue)
            {
                var filterDate = FilterDate.Value.Date;
                query = query.Where(s => s.StartTime.Date == filterDate);
            }

            Schedules = await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Load dropdown data
            var courses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            CourseList = new SelectList(courses, "Id", "Name");

            var professors = await _context.Professors
                .Include(p => p.User)
                .OrderBy(p => p.User.FirstName)
                .ToListAsync();

            ProfessorList = new SelectList(
                professors.Select(p => new
                {
                    p.Id,
                    Name = $"{p.User.FirstName} {p.User.LastName}"
                }),
                "Id",
                "Name"
            );
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound();

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Schedule deleted successfully.";
            return RedirectToPage();
        }
    }
}
