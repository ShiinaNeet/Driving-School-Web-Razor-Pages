using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Subjects
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Subject? Subject { get; set; }
        public IList<CourseSubject> CoursesUsingSubject { get; set; } = new List<CourseSubject>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Subject = await _context.Subjects
                .Include(s => s.Professor)
                    .ThenInclude(p => p!.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Subject == null)
                return NotFound();

            CoursesUsingSubject = await _context.CourseSubjects
                .Include(cs => cs.Course)
                .Where(cs => cs.SubjectId == id)
                .OrderBy(cs => cs.Order)
                .ToListAsync();

            return Page();
        }
    }
}
