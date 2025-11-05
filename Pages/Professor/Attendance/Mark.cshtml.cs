using EnrollmentSystem.Data;
using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnrollmentSystem.Pages.Professor.Attendance
{
    public class MarkModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarkModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Schedule Schedule { get; set; } = null!;
        public IList<StudentAttendance> Students { get; set; } = new List<StudentAttendance>();

        [BindProperty]
        public List<AttendanceInput> AttendanceInputs { get; set; } = new List<AttendanceInput>();

        public class StudentAttendance
        {
            public string StudentId { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public AttendanceStatus? Status { get; set; }
            public int? AttendanceId { get; set; }
            public DateTime? CheckInTime { get; set; }
        }

        public class AttendanceInput
        {
            public string StudentId { get; set; } = string.Empty;
            public AttendanceStatus Status { get; set; }
            public string? Remarks { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            Schedule = await _context.Schedules
                .Include(s => s.Course)
                .Include(s => s.Subject)
                .Include(s => s.Attendances)
                .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(s => s.Id == id && s.ProfessorId == professor.Id);

            if (Schedule == null)
                return NotFound();

            // Get all enrolled students for this course
            var enrolledStudents = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == Schedule.CourseId &&
                           (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Approved))
                .Select(e => e.Student)
                .ToListAsync();

            Students = enrolledStudents.Select(student =>
            {
                var attendance = Schedule.Attendances?.FirstOrDefault(a => a.StudentId == student.Id);
                return new StudentAttendance
                {
                    StudentId = student.Id,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    Email = student.Email!,
                    Status = attendance?.Status,
                    AttendanceId = attendance?.Id,
                    CheckInTime = attendance?.CheckInTime
                };
            }).OrderBy(s => s.StudentName).ToList();

            // Pre-populate attendance inputs
            AttendanceInputs = Students.Select(s => new AttendanceInput
            {
                StudentId = s.StudentId,
                Status = s.Status ?? AttendanceStatus.Absent,
                Remarks = ""
            }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var professor = await _context.Professors
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (professor == null)
                return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id && s.ProfessorId == professor.Id);

            if (schedule == null)
                return NotFound();

            foreach (var input in AttendanceInputs)
            {
                var existingAttendance = schedule.Attendances?
                    .FirstOrDefault(a => a.StudentId == input.StudentId);

                if (existingAttendance != null)
                {
                    // Update existing attendance
                    existingAttendance.Status = input.Status;
                    existingAttendance.Remarks = input.Remarks;
                    existingAttendance.UpdatedAt = DateTime.UtcNow;

                    if (input.Status == AttendanceStatus.Present && existingAttendance.CheckInTime == null)
                    {
                        existingAttendance.CheckInTime = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Create new attendance record
                    var attendance = new EnrollmentSystem.Models.Attendance
                    {
                        ScheduleId = schedule.Id,
                        StudentId = input.StudentId,
                        Status = input.Status,
                        Remarks = input.Remarks,
                        CheckInTime = input.Status == AttendanceStatus.Present ? DateTime.UtcNow : null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Attendances.Add(attendance);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Attendance has been recorded successfully.";
            return RedirectToPage("./Index");
        }
    }
}
