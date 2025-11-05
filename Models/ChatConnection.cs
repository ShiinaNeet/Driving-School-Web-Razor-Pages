using System.ComponentModel.DataAnnotations;

namespace EnrollmentSystem.Models
{
    public class ChatConnection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string ConnectionId { get; set; } = string.Empty;

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
