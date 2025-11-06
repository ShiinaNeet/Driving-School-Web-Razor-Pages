using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Vehicles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Vehicle> Vehicles { get; set; } = new();
        public Dictionary<int, int> VehicleScheduleCounts { get; set; } = new();
        public Dictionary<int, VehicleMaintenance?> UpcomingMaintenance { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vehicles = await _context.Vehicles
                .Include(v => v.Location)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            // Get schedule counts for each vehicle
            var scheduleCounts = await _context.Schedules
                .Where(s => s.VehicleId != null)
                .GroupBy(s => s.VehicleId!.Value)
                .Select(g => new { VehicleId = g.Key, Count = g.Count() })
                .ToListAsync();

            VehicleScheduleCounts = scheduleCounts.ToDictionary(x => x.VehicleId, x => x.Count);

            // Get upcoming maintenance for each vehicle
            var maintenances = await _context.VehicleMaintenances
                .Where(vm => vm.Status == MaintenanceStatus.Scheduled && vm.ScheduledDate >= DateTime.Today)
                .GroupBy(vm => vm.VehicleId)
                .Select(g => g.OrderBy(vm => vm.ScheduledDate).FirstOrDefault())
                .ToListAsync();

            UpcomingMaintenance = maintenances
                .Where(vm => vm != null)
                .ToDictionary(vm => vm!.VehicleId, vm => vm);
        }
    }
}
