using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Locations
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Location> Locations { get; set; } = new();

        public async Task OnGetAsync()
        {
            Locations = await _context.Locations
                .Include(l => l.Vehicles)
                .Include(l => l.Professors)
                .OrderByDescending(l => l.IsHeadquarters)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }
    }
}
