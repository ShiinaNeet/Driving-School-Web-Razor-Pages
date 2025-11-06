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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Vehicle Vehicle { get; set; } = new();

        public SelectList? Locations { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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

            // Check for duplicate license plate
            if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == Vehicle.LicensePlate))
            {
                ModelState.AddModelError("Vehicle.LicensePlate", "A vehicle with this license plate already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            // Check for duplicate VIN
            if (await _context.Vehicles.AnyAsync(v => v.VIN == Vehicle.VIN))
            {
                ModelState.AddModelError("Vehicle.VIN", "A vehicle with this VIN already exists.");
                await LoadSelectListsAsync();
                return Page();
            }

            Vehicle.CreatedAt = DateTime.UtcNow;
            _context.Vehicles.Add(Vehicle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Vehicle {Vehicle.Make} {Vehicle.Model} ({Vehicle.LicensePlate}) has been added successfully.";
            return RedirectToPage("./Index");
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
