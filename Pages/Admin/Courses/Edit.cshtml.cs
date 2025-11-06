using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Courses
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public EditModel(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Course Course { get; set; } = null!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Course = await _context.Courses.FindAsync(id);

            if (Course == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Handle image upload
            if (ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(Course.ImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(Course.ImagePath);
                }

                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(
                    ImageFile,
                    "courses",
                    new[] { ".jpg", ".jpeg", ".png", ".gif" },
                    5242880 // 5MB
                );

                if (success)
                {
                    Course.ImagePath = filePath;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", error);
                    return Page();
                }
            }

            Course.UpdatedAt = DateTime.UtcNow;
            _context.Attach(Course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CourseExistsAsync(Course.Id))
                    return NotFound();
                throw;
            }

            TempData["Message"] = "Course updated successfully.";
            return RedirectToPage("./Index");
        }

        private async Task<bool> CourseExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(e => e.Id == id);
        }
    }
}
