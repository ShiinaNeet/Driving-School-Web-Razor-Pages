using EnrollmentSystem.Models;

namespace EnrollmentSystem.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
        Task SendNotificationToMultipleAsync(IEnumerable<string> userIds, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
        Task SendNotificationToRoleAsync(string role, string title, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int count = 10);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
