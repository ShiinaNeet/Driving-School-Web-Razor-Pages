using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Schedules
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Schedule Schedule { get; set; } = null!;

        public SelectList CourseList { get; set; } = null!;
        public SelectList SubjectList { get; set; } = null!;
        public SelectList ProfessorList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdownData();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return Page();
            }

            // Validate end time is after start time
            if (Schedule.EndTime <= Schedule.StartTime)
            {
                ModelState.AddModelError("Schedule.EndTime", "End time must be after start time.");
                await LoadDropdownData();
                return Page();
            }

            Schedule.CreatedAt = DateTime.UtcNow;
            Schedule.UpdatedAt = DateTime.UtcNow;
            Schedule.Status = ScheduleStatus.Scheduled;

            _context.Schedules.Add(Schedule);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Schedule created successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadDropdownData()
        {
            var courses = await _context.Courses
                .Where(c => c.Status == CourseStatus.Active)
                .OrderBy(c => c.Name)
                .ToListAsync();
            CourseList = new SelectList(courses, "Id", "Name");

            var subjects = await _context.Subjects
                .Where(s => s.Status == SubjectStatus.Active)
                .OrderBy(s => s.Code)
                .ToListAsync();
            SubjectList = new SelectList(
                subjects.Select(s => new
                {
                    s.Id,
                    Name = $"{s.Code} - {s.Name}"
                }),
                "Id",
                "Name"
            );

            var professors = await _context.Professors
                .Include(p => p.User)
                .OrderBy(p => p.User.FirstName)
                .ToListAsync();
            ProfessorList = new SelectList(
                professors.Select(p => new
                {
                    p.Id,
                    Name = $"{p.User.FirstName} {p.User.LastName} ({p.Specialization})"
                }),
                "Id",
                "Name"
            );
        }
    }
}
