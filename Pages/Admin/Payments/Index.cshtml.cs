using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Payments;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Payment> Payments { get; set; } = new();

    public async Task OnGetAsync()
    {
        Payments = await _context.Payments
            .Include(p => p.Student)
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}
