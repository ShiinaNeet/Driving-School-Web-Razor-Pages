using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Guardian;

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
    public List<EnrollmentSystem.Models.Guardian> MyGuardianRecords { get; set; } = new();
    public List<Enrollment> StudentEnrollments { get; set; } = new();
    public List<Payment> StudentPayments { get; set; } = new();

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User) ?? new ApplicationUser();

        MyGuardianRecords = await _context.Guardians
            .Include(g => g.Student)
            .Where(g => g.UserId == CurrentUser.Id)
            .ToListAsync();

        var studentIds = MyGuardianRecords.Select(g => g.StudentId).ToList();

        StudentEnrollments = await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => studentIds.Contains(e.StudentId))
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync();

        StudentPayments = await _context.Payments
            .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
            .Where(p => studentIds.Contains(p.StudentId))
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}
