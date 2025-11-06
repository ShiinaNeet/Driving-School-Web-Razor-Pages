using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Vehicles
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Vehicle Vehicle { get; set; } = null!;
        public List<VehicleMaintenance> MaintenanceRecords { get; set; } = new();
        public List<Schedule> UpcomingSchedules { get; set; } = new();
        public int TotalSchedules { get; set; }
        public decimal TotalMaintenanceCost { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle = await _context.Vehicles
                .Include(v => v.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Vehicle == null)
            {
                return NotFound();
            }

            // Get maintenance records
            MaintenanceRecords = await _context.VehicleMaintenances
                .Where(vm => vm.VehicleId == id)
                .OrderByDescending(vm => vm.ScheduledDate)
                .Take(10)
                .ToListAsync();

            TotalMaintenanceCost = MaintenanceRecords
                .Where(vm => vm.Cost.HasValue)
                .Sum(vm => vm.Cost!.Value);

            // Get upcoming schedules
            UpcomingSchedules = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Professor).ThenInclude(p => p!.User)
                .Where(s => s.VehicleId == id && s.StartTime >= DateTime.Now)
                .OrderBy(s => s.StartTime)
                .Take(5)
                .ToListAsync();

            TotalSchedules = await _context.Schedules
                .Where(s => s.VehicleId == id)
                .CountAsync();

            return Page();
        }
    }
}
