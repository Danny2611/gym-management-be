using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories
{
    public interface IPackageRepository
    {
        Task<List<Package>> GetActiveAsync();
        Task<Package?> GetByIdAsync(string id);
        Task<List<Package>> GetByIdsAsync(List<string> ids);
        Task<PackageDetail?> GetDetailByPackageIdAsync(string packageId);
    }
}
