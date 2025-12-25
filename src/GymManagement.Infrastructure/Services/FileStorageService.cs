// GymManagement.Infrastructure/Services/FileStorageService.cs
using GymManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GymManagement.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadPath;
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;

        public FileStorageService(IConfiguration configuration)
        {
            _uploadPath = configuration["FileStorage:AvatarPath"] ?? "wwwroot/uploads/avatars";
            _maxFileSize = long.Parse(configuration["FileStorage:MaxFileSize"] ?? "5242880"); // 5MB
            _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            _allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size
            if (file.Length > _maxFileSize)
                return false;

            // Check extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Check MIME type
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        public async Task<string> UploadAvatarAsync(IFormFile file, string userId)
        {
            if (!IsValidImageFile(file))
            {
                throw new Exception("Chỉ chấp nhận file ảnh (jpeg, png, gif, webp) với kích thước tối đa 5MB");
            }

            // Create unique filename: userId-timestamp-originalname
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{userId}-{timestamp}{extension}";
            var filePath = Path.Combine(_uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path (để lưu vào database)
            return Path.Combine("uploads/avatars", fileName).Replace("\\", "/");
        }

        public Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return Task.FromResult(false);

                // Construct full path
                var fullPath = Path.Combine("wwwroot", filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa file: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}