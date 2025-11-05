using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Subjects
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public CreateModel(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Subject Subject { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList ProfessorList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
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
                var (success, filePath, error) = await _fileUploadService.UploadFileAsync(
                    ImageFile,
                    "subjects",
                    new[] { ".jpg", ".jpeg", ".png", ".gif" },
                    5242880 // 5MB
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

            Subject.CreatedAt = DateTime.UtcNow;
            _context.Subjects.Add(Subject);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Subject created successfully.";
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
    }
}
