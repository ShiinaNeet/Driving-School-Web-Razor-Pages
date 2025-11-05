namespace EnrollmentSystem.Services
{
    public interface IFileUploadService
    {
        Task<(bool Success, string FilePath, string Error)> UploadFileAsync(IFormFile file, string folder, string[]? allowedExtensions = null, long maxSizeInBytes = 10485760);
        Task<bool> DeleteFileAsync(string filePath);
        string GetFileUrl(string filePath);
        bool FileExists(string filePath);
        (string FileName, string Extension, long Size) GetFileInfo(IFormFile file);
    }
}
