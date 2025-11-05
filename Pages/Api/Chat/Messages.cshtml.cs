using EnrollmentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Api.Chat
{
    [Authorize]
    public class MessagesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MessagesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                           (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    m.Message,
                    m.IsRead,
                    m.SentAt,
                    m.ReadAt
                })
                .ToListAsync();

            // Mark received messages as read
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.SenderId == userId && m.ReceiverId == currentUserId && !m.IsRead)
                .ToListAsync();

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.UtcNow;
            }

            if (unreadMessages.Any())
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(messages);
        }
    }
}
