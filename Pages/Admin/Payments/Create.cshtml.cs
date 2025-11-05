using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Payments;

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
    public Payment Payment { get; set; } = new() { PaymentDate = DateTime.Now };

    public SelectList EnrollmentList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? enrollmentId)
    {
        await LoadEnrollments();

        if (enrollmentId.HasValue)
        {
            Payment.EnrollmentId = enrollmentId.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadEnrollments();
            return Page();
        }

        var enrollment = await _context.Enrollments.FindAsync(Payment.EnrollmentId);
        if (enrollment == null)
        {
            ModelState.AddModelError("", "Enrollment not found.");
            await LoadEnrollments();
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        Payment.StudentId = enrollment.StudentId;
        Payment.RecordedBy = currentUser?.Email ?? "System";
        Payment.Status = PaymentStatus.Completed;
        Payment.CreatedAt = DateTime.UtcNow;

        // Update enrollment balance
        enrollment.PaidAmount += Payment.Amount;
        enrollment.Balance = enrollment.TotalFee - enrollment.PaidAmount;
        enrollment.UpdatedAt = DateTime.UtcNow;

        _context.Payments.Add(Payment);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Payment recorded successfully.";
        return RedirectToPage("./Index");
    }

    private async Task LoadEnrollments()
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved)
            .ToListAsync();

        EnrollmentList = new SelectList(
            enrollments.Select(e => new
            {
                e.Id,
                Display = $"{e.Student.FirstName} {e.Student.LastName} - {e.Course.Name} (Balance: ${e.Balance})"
            }),
            "Id",
            "Display"
        );
    }
}
