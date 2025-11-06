using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Vehicles
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = null!;
        public int ScheduleCount { get; set; }
        public int MaintenanceCount { get; set; }
        public bool HasDependencies { get; set; }

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

            ScheduleCount = await _context.Schedules.CountAsync(s => s.VehicleId == id);
            MaintenanceCount = await _context.VehicleMaintenances.CountAsync(vm => vm.VehicleId == id);
            HasDependencies = ScheduleCount > 0 || MaintenanceCount > 0;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle = await _context.Vehicles.FindAsync(id);

            if (Vehicle == null)
            {
                return NotFound();
            }

            // Check for dependencies
            var hasSchedules = await _context.Schedules.AnyAsync(s => s.VehicleId == id);
            if (hasSchedules)
            {
                TempData["ErrorMessage"] = "Cannot delete this vehicle as it has associated schedules. Please remove or reassign schedules first.";
                return RedirectToPage("./Details", new { id });
            }

            // Delete associated maintenance records
            var maintenanceRecords = await _context.VehicleMaintenances
                .Where(vm => vm.VehicleId == id)
                .ToListAsync();
            _context.VehicleMaintenances.RemoveRange(maintenanceRecords);

            _context.Vehicles.Remove(Vehicle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Vehicle {Vehicle.Make} {Vehicle.Model} ({Vehicle.LicensePlate}) has been deleted successfully.";
            return RedirectToPage("./Index");
        }
    }
}
