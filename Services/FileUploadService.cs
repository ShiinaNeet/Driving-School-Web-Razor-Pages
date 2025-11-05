namespace EnrollmentSystem.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<(bool Success, string FilePath, string Error)> UploadFileAsync(
            IFormFile file,
            string folder,
            string[]? allowedExtensions = null,
            long maxSizeInBytes = 10485760) // Default 10MB
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return (false, string.Empty, "No file provided");
                }

                // Check file size
                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeMB = maxSizeInBytes / 1048576;
                    return (false, string.Empty, $"File size exceeds maximum allowed size of {maxSizeMB}MB");
                }

                // Check extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (allowedExtensions != null && !allowedExtensions.Contains(extension))
                {
                    return (false, string.Empty, $"File type {extension} is not allowed");
                }

                // Create upload directory if it doesn't exist
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for database storage
                var relativePath = Path.Combine("uploads", folder, fileName).Replace("\\", "/");
                return (true, relativePath, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return (false, string.Empty, "Error uploading file");
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(_environment.WebRootPath, filePath.Replace("/", "\\"));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            return $"/{filePath.Replace("\\", "/")}";
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.Replace("/", "\\"));
            return File.Exists(fullPath);
        }

        public (string FileName, string Extension, long Size) GetFileInfo(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var size = file.Length;

            return (fileName, extension, size);
        }
    }
}
