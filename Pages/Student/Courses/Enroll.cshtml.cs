using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using EnrollmentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Student.Courses
{
    public class EnrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public EnrollModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public Course Course { get; set; } = null!;
        public ApplicationUser CurrentUser { get; set; } = null!;
        public int EnrolledCount { get; set; }
        public int AvailableSlots { get; set; }

        [BindProperty]
        public decimal InitialPayment { get; set; }

        [BindProperty]
        public string? Notes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Course = await _context.Courses
                .Include(c => c.CourseSubjects)
                .ThenInclude(cs => cs.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Course == null)
                return NotFound();

            // Check if course is available
            if (Course.Status != CourseStatus.Active)
            {
                TempData["Error"] = "This course is not currently accepting enrollments.";
                return RedirectToPage("./Details", new { id });
            }

            // Get current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CurrentUser = await _userManager.FindByIdAsync(userId!);

            if (CurrentUser == null)
                return NotFound();

            // Check if already enrolled
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == id);

            if (existingEnrollment != null)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToPage("./Details", new { id });
            }

            // Check availability
            EnrolledCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == id &&
                                (e.Status == EnrollmentStatus.Active ||
                                 e.Status == EnrollmentStatus.Approved ||
                                 e.Status == EnrollmentStatus.Pending));

            if (Course.MaxStudents.HasValue)
            {
                AvailableSlots = Course.MaxStudents.Value - EnrolledCount;
                if (AvailableSlots <= 0)
                {
                    TempData["Error"] = "This course is currently full.";
                    return RedirectToPage("./Details", new { id });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validate initial payment
            if (InitialPayment < 0 || InitialPayment > course.TotalFee)
            {
                ModelState.AddModelError("InitialPayment", "Initial payment must be between $0 and the total course fee.");
                return await OnGetAsync(id);
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                StudentId = userId!,
                CourseId = id,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Pending,
                Balance = course.TotalFee - InitialPayment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Create initial payment record if payment was made
            if (InitialPayment > 0)
            {
                var payment = new Payment
                {
                    EnrollmentId = enrollment.Id,
                    Amount = InitialPayment,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = PaymentMethod.Cash, // Default, can be changed by admin
                    Status = PaymentStatus.Pending,
                    Notes = "Initial enrollment payment",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            // Send notification to student
            await _notificationService.SendNotificationAsync(
                userId!,
                "Enrollment Request Submitted",
                $"Your enrollment request for {course.Name} has been submitted and is pending approval.",
                NotificationType.Enrollment
            );

            // Send notification to admins
            await _notificationService.SendNotificationToRoleAsync(
                "Admin",
                "New Enrollment Request",
                $"New enrollment request from {CurrentUser.FirstName} {CurrentUser.LastName} for {course.Name}.",
                NotificationType.Enrollment
            );

            TempData["Message"] = "Your enrollment request has been submitted successfully! You will be notified once it is approved.";
            return RedirectToPage("/Student/Index");
        }
    }
}
