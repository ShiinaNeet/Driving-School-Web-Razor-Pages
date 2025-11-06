using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Overall Statistics
        public int TotalStudents { get; set; }
        public int TotalProfessors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int PendingEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }

        // Financial Statistics
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal PendingPayments { get; set; }

        // Academic Statistics
        public int TotalSchedules { get; set; }
        public int UpcomingSchedules { get; set; }
        public int CompletedSchedules { get; set; }
        public int TotalGradesRecorded { get; set; }
        public decimal AverageGrade { get; set; }
        public int TotalAttendanceRecords { get; set; }
        public int TotalMaterials { get; set; }

        // Recent Activities
        public IList<Enrollment> RecentEnrollments { get; set; } = new List<Enrollment>();
        public IList<Payment> RecentPayments { get; set; } = new List<Payment>();
        public IList<Schedule> UpcomingClasses { get; set; } = new List<Schedule>();

        // Top Courses
        public IList<CourseStats> TopCourses { get; set; } = new List<CourseStats>();

        public class CourseStats
        {
            public Course Course { get; set; } = null!;
            public int EnrollmentCount { get; set; }
            public decimal Revenue { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Get user counts
            TotalStudents = await _context.Students.CountAsync();
            TotalProfessors = await _context.Professors.CountAsync();

            // Get course and subject counts
            TotalCourses = await _context.Courses.CountAsync();
            TotalSubjects = await _context.Subjects.CountAsync();

            // Get enrollment statistics
            var allEnrollments = await _context.Enrollments.ToListAsync();
            TotalEnrollments = allEnrollments.Count;
            ActiveEnrollments = allEnrollments.Count(e => e.Status == EnrollmentStatus.Active);
            PendingEnrollments = allEnrollments.Count(e => e.Status == EnrollmentStatus.Pending);
            CompletedEnrollments = allEnrollments.Count(e => e.Status == EnrollmentStatus.Completed);

            // Financial statistics
            var allPayments = await _context.Payments.ToListAsync();
            TotalRevenue = allEnrollments.Sum(e => e.TotalFee);
            TotalPaid = allPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            TotalOutstanding = allEnrollments.Sum(e => e.Balance);
            PendingPayments = allPayments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount);

            // Academic statistics
            TotalSchedules = await _context.Schedules.CountAsync();
            UpcomingSchedules = await _context.Schedules
                .CountAsync(s => s.StartTime >= DateTime.UtcNow && s.Status == ScheduleStatus.Scheduled);
            CompletedSchedules = await _context.Schedules
                .CountAsync(s => s.Status == ScheduleStatus.Completed);

            var allAssessments = await _context.Assessments.ToListAsync();
            TotalGradesRecorded = allAssessments.Count;
            AverageGrade = allAssessments.Any()
                ? allAssessments.Average(a => (a.Score / a.MaxScore) * 100)
                : 0;

            TotalAttendanceRecords = await _context.Attendances.CountAsync();
            TotalMaterials = await _context.SubjectMaterials.CountAsync();

            // Recent activities
            RecentEnrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(10)
                .ToListAsync();

            RecentPayments = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Enrollment)
                .ThenInclude(e => e.Course)
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .ToListAsync();

            UpcomingClasses = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Professor)
                .ThenInclude(p => p!.User)
                .Where(s => s.StartTime >= DateTime.UtcNow && s.Status == ScheduleStatus.Scheduled)
                .OrderBy(s => s.StartTime)
                .Take(10)
                .ToListAsync();

            // Top courses by enrollment
            var courseEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.Course)
                .Select(g => new
                {
                    Course = g.Key,
                    EnrollmentCount = g.Count(),
                    Revenue = g.Sum(e => e.PaidAmount)
                })
                .OrderByDescending(x => x.EnrollmentCount)
                .Take(5)
                .ToListAsync();

            TopCourses = courseEnrollments.Select(x => new CourseStats
            {
                Course = x.Course,
                EnrollmentCount = x.EnrollmentCount,
                Revenue = x.Revenue
            }).ToList();
        }
    }
}
