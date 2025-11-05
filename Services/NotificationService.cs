using EnrollmentSystem.Data;
using EnrollmentSystem.Hubs;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationService(
            ApplicationDbContext context,
            IHubContext<NotificationHub> hubContext,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task SendNotificationAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send real-time notification via SignalR
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                type = notification.Type.ToString(),
                createdAt = notification.CreatedAt
            });
        }

        public async Task SendNotificationToMultipleAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null)
        {
            foreach (var userId in userIds)
            {
                await SendNotificationAsync(userId, title, message, type, relatedEntityId, relatedEntityType);
            }
        }

        public async Task SendNotificationToRoleAsync(string role, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var userIds = usersInRole.Select(u => u.Id);
            await SendNotificationToMultipleAsync(userIds, title, message, type, relatedEntityId, relatedEntityType);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int count = 10)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
