using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Attendance
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public SelectList CourseList { get; set; } = null!;
        public SelectList ProfessorList { get; set; } = null!;
        public SelectList StudentList { get; set; } = null!;

        [BindProperty(SupportsGet = true)]
        public int? FilterCourseId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterProfessorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDate { get; set; }

        // Statistics
        public int TotalRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }
        public decimal AttendanceRate { get; set; }

        public class AttendanceRecord
        {
            public EnrollmentSystem.Models.Attendance Attendance { get; set; } = null!;
            public Schedule Schedule { get; set; } = null!;
            public ApplicationUser Student { get; set; } = null!;
        }

        public async Task OnGetAsync()
        {
            // Load filter options
            var courses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            CourseList = new SelectList(courses, "Id", "Name");

            var professors = await _context.Professors
                .Include(p => p.User)
                .OrderBy(p => p.User.FirstName)
                .ToListAsync();
            ProfessorList = new SelectList(
                professors.Select(p => new
                {
                    p.Id,
                    Name = $"{p.User.FirstName} {p.User.LastName}"
                }),
                "Id",
                "Name"
            );

            var students = await _context.Students
                .OrderBy(s => s.FirstName)
                .ToListAsync();
            StudentList = new SelectList(
                students.Select(s => new
                {
                    s.Id,
                    Name = $"{s.FirstName} {s.LastName}"
                }),
                "Id",
                "Name"
            );

            // Query attendance with filters
            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Course)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Subject)
                .Include(a => a.Schedule)
                .ThenInclude(s => s.Professor)
                .ThenInclude(p => p!.User)
                .AsQueryable();

            // Apply filters
            if (FilterCourseId.HasValue)
                query = query.Where(a => a.Schedule.CourseId == FilterCourseId.Value);

            if (FilterProfessorId.HasValue)
                query = query.Where(a => a.Schedule.ProfessorId == FilterProfessorId.Value);

            if (!string.IsNullOrEmpty(FilterStudentId))
                query = query.Where(a => a.StudentId == FilterStudentId);

            if (FilterDate.HasValue)
            {
                var filterDate = FilterDate.Value.Date;
                query = query.Where(a => a.Schedule.StartTime.Date == filterDate);
            }

            var attendances = await query
                .OrderByDescending(a => a.Schedule.StartTime)
                .ToListAsync();

            AttendanceRecords = attendances.Select(a => new AttendanceRecord
            {
                Attendance = a,
                Schedule = a.Schedule,
                Student = a.Student
            }).ToList();

            // Calculate statistics
            TotalRecords = attendances.Count;
            PresentCount = attendances.Count(a => a.Status == AttendanceStatus.Present);
            AbsentCount = attendances.Count(a => a.Status == AttendanceStatus.Absent);
            LateCount = attendances.Count(a => a.Status == AttendanceStatus.Late);
            ExcusedCount = attendances.Count(a => a.Status == AttendanceStatus.Excused);
            AttendanceRate = TotalRecords > 0
                ? ((decimal)(PresentCount + LateCount) / TotalRecords) * 100
                : 0;
        }
    }
}
