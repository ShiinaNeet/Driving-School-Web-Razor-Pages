using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
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
    private readonly INotificationService _notificationService;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
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

        // Send notifications
        var student = await _context.Users.FindAsync(enrollment.StudentId);
        var course = await _context.Courses.FindAsync(enrollment.CourseId);

        if (student != null && course != null)
        {
            var studentName = $"{student.FirstName} {student.LastName}";

            // Notify student
            await _notificationService.SendNotificationAsync(
                enrollment.StudentId,
                "Payment Received",
                $"Payment of ${Payment.Amount:N2} has been recorded for {course.Name}. Remaining balance: ${enrollment.Balance:N2}",
                NotificationType.Payment,
                Payment.Id.ToString(),
                "Payment"
            );

            // Notify guardians
            var guardians = await _context.Guardians
                .Where(g => g.StudentId == enrollment.StudentId)
                .Select(g => g.UserId)
                .ToListAsync();

            foreach (var guardianId in guardians)
            {
                await _notificationService.SendNotificationAsync(
                    guardianId,
                    "Student Payment Received",
                    $"Payment of ${Payment.Amount:N2} has been recorded for {studentName}'s enrollment in {course.Name}. Remaining balance: ${enrollment.Balance:N2}",
                    NotificationType.Payment,
                    Payment.Id.ToString(),
                    "Payment"
                );
            }

            // Notify admin who recorded it
            if (currentUser != null)
            {
                await _notificationService.SendNotificationAsync(
                    currentUser.Id,
                    "Payment Recorded",
                    $"Successfully recorded payment of ${Payment.Amount:N2} for {studentName}",
                    NotificationType.Payment,
                    Payment.Id.ToString(),
                    "Payment"
                );
            }
        }

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
