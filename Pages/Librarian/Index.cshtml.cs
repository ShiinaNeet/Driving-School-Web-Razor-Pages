using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Librarian;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int ActiveBorrowings { get; set; }
    public int OverdueBorrowings { get; set; }
    public List<BookBorrowing> RecentBorrowings { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalBooks = await _context.LibraryBooks.CountAsync();
        AvailableBooks = await _context.LibraryBooks.SumAsync(b => b.AvailableCopies);
        ActiveBorrowings = await _context.BookBorrowings.CountAsync(b => b.Status == BorrowingStatus.Active);
        OverdueBorrowings = await _context.BookBorrowings.CountAsync(b =>
            b.Status == BorrowingStatus.Active && b.DueDate < DateTime.Now);

        RecentBorrowings = await _context.BookBorrowings
            .Include(b => b.Book)
            .Include(b => b.Borrower)
            .OrderByDescending(b => b.BorrowDate)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostReturnAsync(int id)
    {
        var borrowing = await _context.BookBorrowings
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (borrowing == null)
        {
            return NotFound();
        }

        borrowing.Status = BorrowingStatus.Returned;
        borrowing.ReturnDate = DateTime.Now;
        borrowing.UpdatedAt = DateTime.UtcNow;

        // Update book availability
        borrowing.Book.AvailableCopies++;

        await _context.SaveChangesAsync();

        TempData["Message"] = "Book return processed successfully.";
        return RedirectToPage();
    }
}
