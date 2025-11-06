using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Student.Materials
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<CourseWithMaterials> CourseMaterials { get; set; } = new List<CourseWithMaterials>();

        public class CourseWithMaterials
        {
            public Course Course { get; set; } = null!;
            public IList<SubjectMaterial> Materials { get; set; } = new List<SubjectMaterial>();
        }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get student's enrolled courses
            var enrolledCourses = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId &&
                           (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved))
                .Select(e => e.Course)
                .ToListAsync();

            CourseMaterials = new List<CourseWithMaterials>();

            foreach (var course in enrolledCourses)
            {
                // Get subjects for this course
                var subjectIds = await _context.CourseSubjects
                    .Where(cs => cs.CourseId == course.Id)
                    .Select(cs => cs.SubjectId)
                    .ToListAsync();

                // Get materials for these subjects (only public materials)
                var materials = await _context.SubjectMaterials
                    .Include(sm => sm.Subject)
                    .Where(sm => subjectIds.Contains(sm.SubjectId) && sm.IsPublic)
                    .OrderByDescending(sm => sm.CreatedAt)
                    .ToListAsync();

                CourseMaterials.Add(new CourseWithMaterials
                {
                    Course = course,
                    Materials = materials
                });
            }
        }
    }
}
