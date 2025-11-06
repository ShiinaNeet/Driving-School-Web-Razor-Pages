using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Subjects
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
        public Subject Subject { get; set; } = null!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList ProfessorList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Subject = await _context.Subjects.FindAsync(id);

            if (Subject == null)
                return NotFound();

            await LoadProfessors();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadProfessors();
                return Page();
            }

            // Handle image upload
            if (ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(Subject.ImagePath))
                {
                    await _fileUploadService.DeleteFileAsync(Subject.ImagePath);
                }

                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(
                    ImageFile,
                    "subjects",
                    new[] { ".jpg", ".jpeg", ".png", ".gif" },
                    5242880
                );

                if (success)
                {
                    Subject.ImagePath = filePath;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", error);
                    await LoadProfessors();
                    return Page();
                }
            }

            Subject.UpdatedAt = DateTime.UtcNow;
            _context.Attach(Subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SubjectExistsAsync(Subject.Id))
                    return NotFound();
                throw;
            }

            TempData["Message"] = "Subject updated successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadProfessors()
        {
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

        private async Task<bool> SubjectExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(e => e.Id == id);
        }
    }
}
