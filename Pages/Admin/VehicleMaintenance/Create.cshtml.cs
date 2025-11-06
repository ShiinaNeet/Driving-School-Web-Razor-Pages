using EnrollmentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.VehicleMaintenance
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.VehicleMaintenance VehicleMaintenance { get; set; } = new();

        public SelectList? Vehicles { get; set; }

        public async Task<IActionResult> OnGetAsync(int? vehicleId)
        {
            await LoadSelectListsAsync();

            if (vehicleId.HasValue)
            {
                VehicleMaintenance.VehicleId = vehicleId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            VehicleMaintenance.CreatedAt = DateTime.UtcNow;
            VehicleMaintenance.PerformedBy = User.Identity?.Name;

            _context.VehicleMaintenances.Add(VehicleMaintenance);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Maintenance record has been created successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
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
