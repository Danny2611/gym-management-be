// GymManagement.Application/Interfaces/Services/IFileStorageService.cs
using Microsoft.AspNetCore.Http;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface IFileStorageService
    {
        Task<string> UploadAvatarAsync(IFormFile file, string userId);
        Task<bool> DeleteFileAsync(string filePath);
        bool IsValidImageFile(IFormFile file);
    }
}