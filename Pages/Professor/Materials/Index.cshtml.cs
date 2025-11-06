using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Materials
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileUploadService fileUploadService)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
        }

        public IList<SubjectMaterial> SubjectMaterials { get; set; } = new List<SubjectMaterial>();
        public IList<Subject> ProfessorSubjects { get; set; } = new List<Subject>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            // Get professor's subjects
            ProfessorSubjects = await _context.Subjects
                .Where(s => s.ProfessorId == professor.Id)
                .OrderBy(s => s.Code)
                .ToListAsync();

            var subjectIds = ProfessorSubjects.Select(s => s.Id).ToList();

            // Get all materials for professor's subjects
            SubjectMaterials = await _context.SubjectMaterials
                .Include(sm => sm.Subject)
                .Where(sm => subjectIds.Contains(sm.SubjectId))
                .OrderByDescending(sm => sm.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var material = await _context.SubjectMaterials.FindAsync(id);
            if (material == null)
                return NotFound();

            // Verify this professor owns the subject
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            var subject = await _context.Subjects.FindAsync(material.SubjectId);
            if (subject == null || subject.ProfessorId != professor.Id)
                return Forbid();

            // Delete the file
            if (!string.IsNullOrEmpty(material.FilePath))
            {
                await _fileUploadService.DeleteFileAsync(material.FilePath);
            }

            _context.SubjectMaterials.Remove(material);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Material deleted successfully.";
            return RedirectToPage();
        }
    }
}
