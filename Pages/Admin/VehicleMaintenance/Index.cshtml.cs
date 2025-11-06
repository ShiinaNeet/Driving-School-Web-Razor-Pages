using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.VehicleMaintenance
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Models.VehicleMaintenance> MaintenanceRecords { get; set; } = new();
        public SelectList? Vehicles { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterVehicleId { get; set; }

        [BindProperty(SupportsGet = true)]
        public MaintenanceStatus? FilterStatus { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.VehicleMaintenances
                .Include(vm => vm.Vehicle)
                .AsQueryable();

            if (FilterVehicleId.HasValue)
            {
                query = query.Where(vm => vm.VehicleId == FilterVehicleId.Value);
            }

            if (FilterStatus.HasValue)
            {
                query = query.Where(vm => vm.Status == FilterStatus.Value);
            }

            MaintenanceRecords = await query
                .OrderByDescending(vm => vm.ScheduledDate)
                .ToListAsync();

            var vehicles = await _context.Vehicles
                .OrderBy(v => v.Make).ThenBy(v => v.Model)
                .ToListAsync();

            Vehicles = new SelectList(
                vehicles.Select(v => new
                {
                    v.Id,
                    Display = $"{v.Year} {v.Make} {v.Model} ({v.LicensePlate})"
                }),
                "Id",
                "Display"
            );
        }
    }
}
