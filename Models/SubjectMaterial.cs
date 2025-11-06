namespace EnrollmentSystem.Models
{
    public class SubjectMaterial
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public string? Description { get; set; }
        public MaterialType Type { get; set; }

        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublic { get; set; } = true;
    }
}
