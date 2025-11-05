using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EnrollmentSystem.Pages.Admin.Courses;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Course Course { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Course.CreatedAt = DateTime.UtcNow;
        _context.Courses.Add(Course);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Course created successfully.";
        return RedirectToPage("./Index");
    }
}
