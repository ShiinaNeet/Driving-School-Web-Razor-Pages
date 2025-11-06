using EnrollmentSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnrollmentSystem.Services
{
    public interface IGdprService
    {
        Task<byte[]> ExportUserDataAsync(string userId);
        Task<bool> DeleteUserDataAsync(string userId);
        Task<bool> AnonymizeUserDataAsync(string userId);
    }

    public class GdprService : IGdprService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GdprService> _logger;

        public GdprService(ApplicationDbContext context, ILogger<GdprService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<byte[]> ExportUserDataAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Enrollments).ThenInclude(e => e.Course)
                .Include(u => u.Payments)
                .Include(u => u.Attendances)
                .Include(u => u.Certificates)
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var userData = new
            {
                PersonalInfo = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone,
                    user.Address,
                    user.DateOfBirth,
                    user.CreatedAt
                },
                Enrollments = user.Enrollments.Select(e => new
                {
                    e.Course.Name,
                    e.EnrollmentDate,
                    e.Status,
                    e.TotalFee,
                    e.PaidAmount
                }),
                Payments = user.Payments.Select(p => new
                {
                    p.Amount,
                    p.PaymentDate,
                    p.Status,
                    p.PaymentMethod
                }),
                Attendance = user.Attendances.Select(a => new
                {
                    a.Date,
                    a.Status
                }),
                Certificates = user.Certificates.Select(c => new
                {
                    c.Title,
                    c.IssueDate,
                    c.CertificateNumber
                })
            };

            var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        public async Task<bool> DeleteUserDataAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Note: In production, consider soft delete or anonymization instead of hard delete
            _logger.LogWarning($"Deleting user data for {userId}");

            // Delete related data (respecting foreign key constraints)
            var certificates = await _context.Certificates.Where(c => c.StudentId == userId).ToListAsync();
            _context.Certificates.RemoveRange(certificates);

            var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
            _context.Notifications.RemoveRange(notifications);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnonymizeUserDataAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.FirstName = "DELETED";
            user.LastName = "USER";
            user.Email = $"deleted_{Guid.NewGuid().ToString("N").Substring(0, 8)}@anonymized.local";
            user.Phone = null;
            user.Address = null;
            user.DateOfBirth = null;
            user.ProfilePhoto = null;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Anonymized user data for {userId}");
            return true;
        }
    }
}
