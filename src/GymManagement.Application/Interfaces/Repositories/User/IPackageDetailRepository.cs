using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IPackageDetailRepository
    {
        Task<List<PackageDetail>> GetByPackageIdsAsync(List<string> packageIds);
        Task<PackageDetail> GetByIdAsync(string id);
        Task<PackageDetail> GetByPackageIdAsync(string packageId);
    }
}
