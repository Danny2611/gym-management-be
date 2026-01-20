using GymManagement.Domain.Entities;

public interface IPackageService
{
    Task<List<Package>> GetPackagesAsync();
    Task<object?> GetPackageByIdAsync(String packageId);
}
