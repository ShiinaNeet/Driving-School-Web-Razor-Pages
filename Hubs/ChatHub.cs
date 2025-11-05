using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add connection to database
                var connection = new ChatConnection
                {
                    UserId = userId,
                    ConnectionId = Context.ConnectionId,
                    ConnectedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ChatConnections.Add(connection);
                await _context.SaveChangesAsync();

                // Notify others that user is online
                await Clients.Others.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Remove connection from database
                var connection = await _context.ChatConnections
                    .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

                if (connection != null)
                {
                    connection.IsActive = false;
                    await _context.SaveChangesAsync();
                }

                // Check if user has any other active connections
                var hasOtherConnections = await _context.ChatConnections
                    .AnyAsync(c => c.UserId == userId && c.IsActive && c.ConnectionId != Context.ConnectionId);

                if (!hasOtherConnections)
                {
                    // Notify others that user is offline
                    await Clients.Others.SendAsync("UserOffline", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId))
                return;

            // Save message to database
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Get receiver's connection IDs
            var receiverConnections = await _context.ChatConnections
                .Where(c => c.UserId == receiverId && c.IsActive)
                .Select(c => c.ConnectionId)
                .ToListAsync();

            // Get sender info
            var sender = await _context.Users.FindAsync(senderId);
            var senderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown";

            // Send to receiver(s)
            foreach (var connectionId in receiverConnections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", new
                {
                    id = chatMessage.Id,
                    senderId = senderId,
                    senderName = senderName,
                    message = message,
                    sentAt = chatMessage.SentAt
                });
            }

            // Confirm to sender
            await Clients.Caller.SendAsync("MessageSent", new
            {
                id = chatMessage.Id,
                receiverId = receiverId,
                message = message,
                sentAt = chatMessage.SentAt
            });
        }

        public async Task MarkAsRead(int messageId)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);

            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Notify sender that message was read
                var senderConnections = await _context.ChatConnections
                    .Where(c => c.UserId == message.SenderId && c.IsActive)
                    .Select(c => c.ConnectionId)
                    .ToListAsync();

                foreach (var connectionId in senderConnections)
                {
                    await Clients.Client(connectionId).SendAsync("MessageRead", messageId);
                }
            }
        }

        public async Task<bool> IsUserOnline(string userId)
        {
            var isOnline = await _context.ChatConnections
                .AnyAsync(c => c.UserId == userId && c.IsActive);

            return isOnline;
        }
    }
}
