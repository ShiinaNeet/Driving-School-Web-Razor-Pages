using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnrollmentSystem.Models;

public class BookBorrowing
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    public string BorrowerId { get; set; } = string.Empty;

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public BorrowingStatus Status { get; set; } = BorrowingStatus.Active;

    [Column(TypeName = "decimal(10,2)")]
    public decimal? LateFee { get; set; }

    public string? Remarks { get; set; }

    public string? IssuedBy { get; set; }

    public string? ReceivedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("BookId")]
    public LibraryBook Book { get; set; } = null!;

    [ForeignKey("BorrowerId")]
    public ApplicationUser Borrower { get; set; } = null!;
}

public enum BorrowingStatus
{
    Active,
    Returned,
    Overdue,
    Lost
}
