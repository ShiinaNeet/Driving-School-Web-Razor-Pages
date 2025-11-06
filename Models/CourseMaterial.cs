namespace EnrollmentSystem.Models
{
    public class CourseMaterial
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; // pdf, docx, video, etc.
        public long FileSize { get; set; } // in bytes

        public string? Description { get; set; }
        public MaterialType Type { get; set; }

        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublic { get; set; } = true; // visible to enrolled students
    }

    public enum MaterialType
    {
        Syllabus,
        Lecture,
        Assignment,
        Video,
        Document,
        Other
    }
}
