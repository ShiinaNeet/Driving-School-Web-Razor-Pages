using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Subject> Subjects { get; set; } = new List<Subject>();

        public async Task OnGetAsync()
        {
            Subjects = await _context.Subjects
                .Include(s => s.Professor)
                    .ThenInclude(p => p!.User)
                .OrderBy(s => s.Code)
                .ToListAsync();
        }
    }
}
