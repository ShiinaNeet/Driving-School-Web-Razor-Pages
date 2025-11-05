using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Student;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public ApplicationUser CurrentUser { get; set; } = null!;
    public List<Enrollment> MyEnrollments { get; set; } = new();
    public List<Payment> MyPayments { get; set; } = new();
    public decimal TotalPaid { get; set; }
    public decimal TotalBalance { get; set; }

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User) ?? new ApplicationUser();

        MyEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == CurrentUser.Id)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();

        MyPayments = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
            .Where(p => p.StudentId == CurrentUser.Id)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        TotalPaid = MyPayments.Sum(p => p.Amount);
        TotalBalance = MyEnrollments.Sum(e => e.Balance);
    }
}
