using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Api.Chat
{
    [Authorize]
    public class ContactsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser!);
            var currentUserRole = currentUserRoles.FirstOrDefault() ?? "Student";

            List<object> contacts = new();

            // Staff can chat with students and guardians
            if (currentUserRole == "Admin" || currentUserRole == "Professor" || currentUserRole == "Librarian")
            {
                // Get students
                var students = await _userManager.GetUsersInRoleAsync("Student");
                foreach (var student in students)
                {
                    var isOnline = await _context.ChatConnections
                        .AnyAsync(c => c.UserId == student.Id && c.IsActive);

                    contacts.Add(new
                    {
                        userId = student.Id,
                        name = $"{student.FirstName} {student.LastName}",
                        role = "Student",
                        isOnline
                    });
                }

                // Get guardians
                var guardians = await _userManager.GetUsersInRoleAsync("Guardian");
                foreach (var guardian in guardians)
                {
                    var isOnline = await _context.ChatConnections
                        .AnyAsync(c => c.UserId == guardian.Id && c.IsActive);

                    contacts.Add(new
                    {
                        userId = guardian.Id,
                        name = $"{guardian.FirstName} {guardian.LastName}",
                        role = "Guardian",
                        isOnline
                    });
                }
            }
            // Students and guardians can chat with staff
            else
            {
                // Get all staff
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var professors = await _userManager.GetUsersInRoleAsync("Professor");
                var librarians = await _userManager.GetUsersInRoleAsync("Librarian");

                var staff = admins.Concat(professors).Concat(librarians);

                foreach (var staffMember in staff)
                {
                    var roles = await _userManager.GetRolesAsync(staffMember);
                    var isOnline = await _context.ChatConnections
                        .AnyAsync(c => c.UserId == staffMember.Id && c.IsActive);

                    contacts.Add(new
                    {
                        userId = staffMember.Id,
                        name = $"{staffMember.FirstName} {staffMember.LastName}",
                        role = roles.FirstOrDefault() ?? "Staff",
                        isOnline
                    });
                }
            }

            return new JsonResult(contacts);
        }
    }
}
