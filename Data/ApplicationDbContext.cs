using EnrollmentSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Guardian> Guardians { get; set; }
    public DbSet<Professor> Professors { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<CourseSubject> CourseSubjects { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<LibraryBook> LibraryBooks { get; set; }
    public DbSet<BookBorrowing> BookBorrowings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatConnection> ChatConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Guardian relationships
        modelBuilder.Entity<Guardian>()
            .HasOne(g => g.User)
            .WithMany(u => u.GuardiansAsUser)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Guardian>()
            .HasOne(g => g.Student)
            .WithMany(u => u.GuardiansAsStudent)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Professor relationship
        modelBuilder.Entity<Professor>()
            .HasOne(p => p.User)
            .WithOne(u => u.Professor)
            .HasForeignKey<Professor>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Professor>()
            .HasIndex(p => p.EmployeeId)
            .IsUnique();

        // Configure Course
        modelBuilder.Entity<Course>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // Configure Subject
        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Professor)
            .WithMany(p => p.Subjects)
            .HasForeignKey(s => s.ProfessorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure CourseSubject (Many-to-Many)
        modelBuilder.Entity<CourseSubject>()
            .HasOne(cs => cs.Course)
            .WithMany(c => c.CourseSubjects)
            .HasForeignKey(cs => cs.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CourseSubject>()
            .HasOne(cs => cs.Subject)
            .WithMany(s => s.CourseSubjects)
            .HasForeignKey(cs => cs.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Enrollment
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Payment
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Enrollment)
            .WithMany(e => e.Payments)
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure LibraryBook
        modelBuilder.Entity<LibraryBook>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        // Configure BookBorrowing
        modelBuilder.Entity<BookBorrowing>()
            .HasOne(bb => bb.Book)
            .WithMany(b => b.Borrowings)
            .HasForeignKey(bb => bb.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookBorrowing>()
            .HasOne(bb => bb.Borrower)
            .WithMany(u => u.BookBorrowings)
            .HasForeignKey(bb => bb.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Notification
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ChatMessage
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(cm => cm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(cm => cm.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Create index on ChatConnection for quick lookup
        modelBuilder.Entity<ChatConnection>()
            .HasIndex(cc => cc.UserId);

        modelBuilder.Entity<ChatConnection>()
            .HasIndex(cc => cc.ConnectionId);
    }
}
