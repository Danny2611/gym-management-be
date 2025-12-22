using Microsoft.AspNetCore.Http;
public interface IFileStorage
{
    Task<(string Url, string PublicId)> UploadAsync(IFormFile file);
    Task<bool> DeleteAsync(string publicId);
}
