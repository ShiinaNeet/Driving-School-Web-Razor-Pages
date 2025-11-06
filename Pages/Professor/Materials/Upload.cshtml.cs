using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Materials
{
    public class UploadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public UploadModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        [BindProperty]
        public SubjectMaterial Material { get; set; } = null!;

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public SelectList SubjectList { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            await LoadSubjects(professor.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || UploadFile == null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var professor = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == userId);
                if (professor != null)
                    await LoadSubjects(professor.Id);

                if (UploadFile == null)
                    ModelState.AddModelError("UploadFile", "Please select a file to upload.");

                return Page();
            }

            // Verify professor owns the subject
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentProfessor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == currentUserId);

            if (currentProfessor == null)
                return NotFound();

            var subject = await _context.Subjects.FindAsync(Material.SubjectId);
            if (subject == null || subject.ProfessorId != currentProfessor.Id)
            {
                ModelState.AddModelError("", "Invalid subject selection.");
                await LoadSubjects(currentProfessor.Id);
                return Page();
            }

            // Upload file
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".mp4", ".avi", ".mov", ".zip" };
            var maxSize = 52428800; // 50MB

            var (success, filePath, error) = await _fileUploadService.UploadFileAsync(
                UploadFile,
                "materials",
                allowedExtensions,
                maxSize
            );

            if (!success)
            {
                ModelState.AddModelError("UploadFile", error);
                await LoadSubjects(currentProfessor.Id);
                return Page();
            }

            Material.FileName = UploadFile.FileName;
            Material.FilePath = filePath;
            Material.CreatedAt = DateTime.UtcNow;
            Material.UpdatedAt = DateTime.UtcNow;

            _context.SubjectMaterials.Add(Material);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Material uploaded successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadSubjects(int professorId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.ProfessorId == professorId)
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
        }
    }
}
