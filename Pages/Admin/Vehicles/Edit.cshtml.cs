using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Vehicles
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = null!;

        public SelectList? Locations { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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

            await LoadSelectListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            // Check for duplicate license plate (excluding current vehicle)
            if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == Vehicle.LicensePlate && v.Id != Vehicle.Id))
            {
                ModelState.AddModelError("Vehicle.LicensePlate", "A vehicle with this license plate already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Check for duplicate VIN (excluding current vehicle)
            if (await _context.Vehicles.AnyAsync(v => v.VIN == Vehicle.VIN && v.Id != Vehicle.Id))
            {
                ModelState.AddModelError("Vehicle.VIN", "A vehicle with this VIN already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            Vehicle.UpdatedAt = DateTime.UtcNow;
            _context.Attach(Vehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await VehicleExistsAsync(Vehicle.Id))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["SuccessMessage"] = $"Vehicle {Vehicle.Make} {Vehicle.Model} has been updated successfully.";
            return RedirectToPage("./Index");
        }

        private async Task<bool> VehicleExistsAsync(int id)
        {
            return await _context.Vehicles.AnyAsync(e => e.Id == id);
        }

        private async Task LoadSelectListsAsync()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .ToListAsync();
            Locations = new SelectList(locations, "Id", "Name");
        }
    }
}
