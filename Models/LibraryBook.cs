using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class LibraryBook
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Author { get; set; }

    [StringLength(100)]
    public string? Publisher { get; set; }

    public int? PublicationYear { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }

    public int TotalCopies { get; set; } = 1;

    public int AvailableCopies { get; set; } = 1;

    public BookStatus Status { get; set; } = BookStatus.Available;

    public string? CoverImage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<BookBorrowing> Borrowings { get; set; } = new List<BookBorrowing>();
}

public enum BookStatus
{
    Available,
    OutOfStock,
    Archived
}
