using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace EnrollmentSystem.Data;

public class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        string[] roles = { "Admin", "Librarian", "Student", "Guardian", "Professor" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        if (!userManager.Users.Any(u => u.Email == "admin@drivingschool.com"))
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@drivingschool.com",
                Email = "admin@drivingschool.com",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Librarian User
        if (!userManager.Users.Any(u => u.Email == "librarian@drivingschool.com"))
        {
            var librarianUser = new ApplicationUser
            {
                UserName = "librarian@drivingschool.com",
                Email = "librarian@drivingschool.com",
                FirstName = "Jane",
                LastName = "Smith",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(librarianUser, "Librarian@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(librarianUser, "Librarian");
            }
        }

        // Seed Sample Professor
        if (!userManager.Users.Any(u => u.Email == "professor@drivingschool.com"))
        {
            var professorUser = new ApplicationUser
            {
                UserName = "professor@drivingschool.com",
                Email = "professor@drivingschool.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(professorUser, "Professor@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(professorUser, "Professor");

                var professor = new Professor
                {
                    UserId = professorUser.Id,
                    EmployeeId = "PROF001",
                    Specialization = "Defensive Driving",
                    Qualification = "Master in Driver Education",
                    HireDate = DateTime.UtcNow.AddYears(-2),
                    EmploymentStatus = EmploymentStatus.FullTime
                };
                context.Professors.Add(professor);
            }
        }

        // Seed Sample Student
        if (!userManager.Users.Any(u => u.Email == "student@drivingschool.com"))
        {
            var studentUser = new ApplicationUser
            {
                UserName = "student@drivingschool.com",
                Email = "student@drivingschool.com",
                FirstName = "Alice",
                LastName = "Johnson",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                DateOfBirth = new DateTime(2000, 1, 1),
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(studentUser, "Student@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        // Seed Sample Guardian
        if (!userManager.Users.Any(u => u.Email == "guardian@drivingschool.com"))
        {
            var guardianUser = new ApplicationUser
            {
                UserName = "guardian@drivingschool.com",
                Email = "guardian@drivingschool.com",
                FirstName = "Robert",
                LastName = "Johnson",
                EmailConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(guardianUser, "Guardian@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(guardianUser, "Guardian");

                var studentUser = await userManager.FindByEmailAsync("student@drivingschool.com");
                if (studentUser != null)
                {
                    var guardian = new Guardian
                    {
                        UserId = guardianUser.Id,
                        StudentId = studentUser.Id,
                        Relationship = RelationshipType.Father,
                        IsPrimary = true,
                        EmergencyContact = "+1234567890"
                    };
                    context.Guardians.Add(guardian);
                }
            }
        }

        // Seed Sample Courses
        if (!context.Courses.Any())
        {
            var courses = new List<Course>
            {
                new Course
                {
                    Code = "BDC-01",
                    Name = "Basic Driving Course",
                    Description = "Comprehensive basic driving course for beginners",
                    DurationWeeks = 8,
                    TotalFee = 500.00m,
                    MaxStudents = 30,
                    Status = CourseStatus.Active
                },
                new Course
                {
                    Code = "ADC-01",
                    Name = "Advanced Driving Course",
                    Description = "Advanced techniques and defensive driving",
                    DurationWeeks = 6,
                    TotalFee = 700.00m,
                    MaxStudents = 20,
                    Status = CourseStatus.Active
                },
                new Course
                {
                    Code = "CDC-01",
                    Name = "Commercial Driver License Course",
                    Description = "Professional commercial driving certification",
                    DurationWeeks = 12,
                    TotalFee = 1200.00m,
                    MaxStudents = 15,
                    Status = CourseStatus.Active
                }
            };
            context.Courses.AddRange(courses);
        }

        // Seed Sample Subjects
        if (!context.Subjects.Any())
        {
            var subjects = new List<Subject>
            {
                new Subject
                {
                    Code = "TRT-101",
                    Name = "Traffic Rules and Regulations",
                    Description = "Understanding traffic laws and road signs",
                    Credits = 3,
                    Status = SubjectStatus.Active
                },
                new Subject
                {
                    Code = "VOP-101",
                    Name = "Vehicle Operation",
                    Description = "Basic vehicle operation and control",
                    Credits = 4,
                    Status = SubjectStatus.Active
                },
                new Subject
                {
                    Code = "DFD-201",
                    Name = "Defensive Driving",
                    Description = "Advanced defensive driving techniques",
                    Credits = 3,
                    Status = SubjectStatus.Active
                }
            };
            context.Subjects.AddRange(subjects);
        }

        // Seed Sample Library Books
        if (!context.LibraryBooks.Any())
        {
            var books = new List<LibraryBook>
            {
                new LibraryBook
                {
                    ISBN = "978-0-123456-78-9",
                    Title = "The Complete Guide to Driving",
                    Author = "James Smith",
                    Publisher = "Driving Publications",
                    PublicationYear = 2022,
                    Category = "Education",
                    TotalCopies = 5,
                    AvailableCopies = 5,
                    Status = BookStatus.Available
                },
                new LibraryBook
                {
                    ISBN = "978-0-987654-32-1",
                    Title = "Road Safety Handbook",
                    Author = "Mary Johnson",
                    Publisher = "Safety First Press",
                    PublicationYear = 2023,
                    Category = "Safety",
                    TotalCopies = 3,
                    AvailableCopies = 3,
                    Status = BookStatus.Available
                }
            };
            context.LibraryBooks.AddRange(books);
        }

        await context.SaveChangesAsync();
    }
}
