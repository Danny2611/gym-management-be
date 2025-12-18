using GymManagement.Domain.Entities;

public interface IPackageRepository
{
    Task<List<Package>> GetActiveAsync();
    Task<Package?> GetByIdAsync(Guid id);
    Task<PackageDetail?> GetDetailByPackageIdAsync(Guid packageId);
}
