# Driving School Enrollment System

A comprehensive enrollment management system built with ASP.NET Core Razor Pages, featuring multiple user portals for school administrators, librarians, students, and guardians.

## Features

### Multi-Portal System
- **Admin Portal**: Complete school management
- **Student Portal**: View courses, enrollments, and payment history
- **Guardian Portal**: Monitor student progress and payments
- **Librarian Portal**: Manage library books and borrowings

### Core Functionality

#### Course Management
- Create and manage driving courses
- Define course duration, fees, and capacity
- Track course status (Active, Inactive, Archived)
- Assign subjects to courses
- Set prerequisites

#### Subject Management
- Create and organize subjects
- Assign professors to subjects
- Track credit hours
- Manage subject availability

#### Professor Management
- Maintain professor profiles
- Track employee information
- Monitor specializations and qualifications
- Assign subjects to professors

#### Enrollment System
- Student course enrollment
- Enrollment status tracking (Pending, Approved, Active, Completed, Cancelled, Suspended)
- Fee calculation and balance tracking
- Enrollment history

#### Payment Management (Manual Recording)
- Record payments manually
- Multiple payment methods (Cash, Check, Bank Transfer, Credit/Debit Card)
- Payment history tracking
- Balance calculation
- Reference number tracking
- Payment status management

#### Library System
- Book catalog management
- ISBN tracking
- Book availability monitoring
- Borrowing and returns
- Overdue tracking
- Late fee calculation

#### Guardian System
- Link guardians to students
- Relationship tracking (Father, Mother, Guardian, Sibling, Other)
- Primary guardian designation
- Emergency contact information
- View student enrollments and payments

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5.3
- **Validation**: jQuery Validation

## Database Schema

### Core Tables

- **ApplicationUser**: User accounts with profile information
- **Guardians**: Guardian-student relationships
- **Professors**: Professor profiles and employment details
- **Courses**: Course catalog
- **Subjects**: Subject definitions
- **CourseSubjects**: Many-to-many relationship between courses and subjects
- **Enrollments**: Student course enrollments
- **Payments**: Payment history records
- **LibraryBooks**: Book catalog
- **BookBorrowings**: Library borrowing records

## User Roles

1. **Admin**
   - Full system access
   - Manage courses, subjects, professors
   - Manage student enrollments
   - Record and view payments
   - View all system data

2. **Librarian**
   - Manage library books
   - Process book borrowings and returns
   - Track overdue books
   - Calculate late fees

3. **Student**
   - View enrolled courses
   - Check payment history
   - View account balance
   - Update profile

4. **Guardian**
   - View linked students
   - Monitor student enrollments
   - Check student payment history
   - View outstanding balances

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Driving-School-Web-Razor-Pages
   ```

2. **Update connection string**

   Edit `appsettings.json` and update the connection string if needed:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DrivingSchoolEnrollment;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

   Note: The database will be automatically migrated and seeded when the application first runs.

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**

   Open your browser and navigate to `https://localhost:5001` (or the URL shown in the console)

### Demo Accounts

The system comes pre-seeded with demo accounts for testing:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@drivingschool.com | Admin@123 |
| Librarian | librarian@drivingschool.com | Librarian@123 |
| Student | student@drivingschool.com | Student@123 |
| Guardian | guardian@drivingschool.com | Guardian@123 |
| Professor | professor@drivingschool.com | Professor@123 |

## Project Structure

```
EnrollmentSystem/
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                 # Database seeding
├── Models/
│   ├── ApplicationUser.cs          # User model
│   ├── Course.cs                   # Course model
│   ├── Subject.cs                  # Subject model
│   ├── Professor.cs                # Professor model
│   ├── Enrollment.cs               # Enrollment model
│   ├── Payment.cs                  # Payment model
│   ├── Guardian.cs                 # Guardian model
│   ├── LibraryBook.cs              # Library book model
│   └── BookBorrowing.cs            # Borrowing model
├── Pages/
│   ├── Account/                    # Authentication pages
│   ├── Admin/                      # Admin portal pages
│   ├── Student/                    # Student portal pages
│   ├── Guardian/                   # Guardian portal pages
│   ├── Librarian/                  # Librarian portal pages
│   └── Shared/                     # Shared layouts
├── wwwroot/                        # Static files
├── appsettings.json                # Configuration
└── Program.cs                      # Application entry point
```

## Key Features Explained

### Payment System (Manual)

The payment system is designed for manual payment recording:
- Admin staff records payments received offline
- Multiple payment methods supported
- Reference numbers for tracking
- Automatic balance calculation
- Complete payment history

### Enrollment Workflow

1. Admin creates course enrollment for student
2. Total fee is set based on course fee
3. Enrollment starts in "Pending" status
4. Admin approves enrollment (status changes to "Approved" or "Active")
5. Payments are recorded as received
6. Balance is automatically calculated
7. When balance reaches zero, enrollment can be marked "Completed"

### Library System

- Books tracked by ISBN
- Multiple copies per book supported
- Automatic availability calculation
- Borrowing period tracking
- Overdue detection
- Return processing updates book availability

## Security Features

- Password hashing with ASP.NET Identity
- Role-based authorization
- Anti-forgery token protection
- Secure session management
- SQL injection protection via EF Core

## Future Enhancements

Potential features for future releases:
- Online payment integration
- Email notifications
- Report generation (PDF/Excel)
- Student performance tracking
- Attendance management
- Certificate generation
- SMS notifications
- Calendar/scheduling system
- Document upload for students
- Grade management

## Support

For issues or questions, please create an issue in the GitHub repository.

## License

This project is licensed under the MIT License.
