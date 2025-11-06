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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Assessment Assessment { get; set; } = null!;

        [BindProperty]
        public int EnrollmentId { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
        public SelectList SubjectList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? enrollmentId, int? courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            if (enrollmentId.HasValue)
            {
                Enrollment = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .ThenInclude(c => c.CourseSubjects)
                    .ThenInclude(cs => cs.Subject)
                    .FirstOrDefaultAsync(e => e.Id == enrollmentId.Value);

                if (Enrollment == null)
                    return NotFound();

                EnrollmentId = Enrollment.Id;

                // Get subjects for this course that are taught by this professor
                var professorSubjectIds = await _context.Subjects
                    .Where(s => s.ProfessorId == professor.Id)
                    .Select(s => s.Id)
                    .ToListAsync();

                var courseSubjects = Enrollment.Course.CourseSubjects
                    .Where(cs => professorSubjectIds.Contains(cs.SubjectId))
                    .Select(cs => cs.Subject)
                    .OrderBy(s => s.Code)
                    .ToList();

                SubjectList = new SelectList(courseSubjects, "Id", "Name");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(EnrollmentId, null);
            }

            var enrollment = await _context.Enrollments.FindAsync(EnrollmentId);
            if (enrollment == null)
                return NotFound();

            Assessment.EnrollmentId = EnrollmentId;
            Assessment.CreatedAt = DateTime.UtcNow;
            Assessment.UpdatedAt = DateTime.UtcNow;

            // Determine if passed based on score percentage
            var percentage = (Assessment.Score / Assessment.MaxScore) * 100;
            Assessment.Passed = percentage >= 60; // 60% passing grade

            _context.Assessments.Add(Assessment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Assessment grade added successfully.";
            return RedirectToPage("./Index");
        }
    }
}
