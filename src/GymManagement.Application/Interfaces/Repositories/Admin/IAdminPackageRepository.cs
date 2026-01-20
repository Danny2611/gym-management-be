using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminPackageRepository
    {
        Task<(List<Package> packages, int totalCount)> GetAllAsync(PackageQueryOptions options);
        Task<Package> GetByIdAsync(string packageId);
        Task<PackageDetail> GetDetailByPackageIdAsync(string packageId);
        Task<Package> CreateAsync(Package package);
        Task<PackageDetail> CreateDetailAsync(PackageDetail packageDetail);
        Task<Package> UpdateAsync(string packageId, Package package);
        Task<PackageDetail> UpdateDetailAsync(string packageId, PackageDetail packageDetail);
        Task<bool> SoftDeleteAsync(string packageId);
        Task<Package> ToggleStatusAsync(string packageId);
        Task<PackageStatsDto> GetStatsAsync();
    }
}