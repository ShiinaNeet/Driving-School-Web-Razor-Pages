using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Subjects
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public DeleteModel(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Subject Subject { get; set; } = null!;

        public int CourseCount { get; set; }

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

            CourseCount = await _context.CourseSubjects
                .CountAsync(cs => cs.SubjectId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Subject = await _context.Subjects.FindAsync(id);

            if (Subject != null)
            {
                // Delete image if exists
                if (!string.IsNullOrEmpty(Subject.ImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(Subject.ImagePath);
                }

                _context.Subjects.Remove(Subject);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Subject deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
