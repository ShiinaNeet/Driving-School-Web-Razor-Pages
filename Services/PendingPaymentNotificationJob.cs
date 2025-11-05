using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Services
{
    public class PendingPaymentNotificationJob
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PendingPaymentNotificationJob(
            ApplicationDbContext context,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task CheckPendingPaymentsAsync()
        {
            // Get all enrollments with pending balances
            var enrollmentsWithBalance = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Balance > 0 &&
                           (e.Status == EnrollmentStatus.Active ||
                            e.Status == EnrollmentStatus.Approved))
                .ToListAsync();

            // Get all admins
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var enrollment in enrollmentsWithBalance)
            {
                // Calculate days since enrollment
                var daysSinceEnrollment = (DateTime.UtcNow - enrollment.EnrollmentDate).Days;

                // Send notification to admins if balance is pending for more than 7 days
                if (daysSinceEnrollment > 7)
                {
                    var studentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}";
                    var message = $"Student {studentName} has a pending balance of ${enrollment.Balance:N2} for course {enrollment.Course.Name}. " +
                                 $"Enrollment date: {enrollment.EnrollmentDate:d}";

                    foreach (var admin in admins)
                    {
                        await _notificationService.SendNotificationAsync(
                            admin.Id,
                            "Pending Payment Alert",
                            message,
                            NotificationType.Payment,
                            enrollment.Id.ToString(),
                            "Enrollment"
                        );
                    }
                }

                // Also send reminder to student if balance is pending
                if (daysSinceEnrollment % 7 == 0 && daysSinceEnrollment > 0) // Every 7 days
                {
                    var message = $"You have a pending balance of ${enrollment.Balance:N2} for course {enrollment.Course.Name}. " +
                                 $"Please make a payment as soon as possible.";

                    await _notificationService.SendNotificationAsync(
                        enrollment.StudentId,
                        "Payment Reminder",
                        message,
                        NotificationType.Payment,
                        enrollment.Id.ToString(),
                        "Enrollment"
                    );

                    // Also notify guardians if student is a minor or has guardians
                    var guardians = await _context.Guardians
                        .Where(g => g.StudentId == enrollment.StudentId)
                        .Select(g => g.UserId)
                        .ToListAsync();

                    foreach (var guardianId in guardians)
                    {
                        var studentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}";
                        var guardianMessage = $"Student {studentName} has a pending balance of ${enrollment.Balance:N2} for course {enrollment.Course.Name}.";

                        await _notificationService.SendNotificationAsync(
                            guardianId,
                            "Student Payment Reminder",
                            guardianMessage,
                            NotificationType.Payment,
                            enrollment.Id.ToString(),
                            "Enrollment"
                        );
                    }
                }
            }
        }
    }
}
