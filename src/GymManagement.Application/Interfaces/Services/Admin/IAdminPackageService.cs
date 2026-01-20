using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminPackageService
    {
        Task<PackageListResponseDto> GetAllPackagesAsync(PackageQueryOptions options);
        Task<PackageResponseDto> GetPackageByIdAsync(string packageId);
        Task<PackageResponseDto> CreatePackageAsync(CreatePackageDto dto);
        Task<PackageResponseDto> UpdatePackageAsync(string packageId, UpdatePackageDto dto);
        Task<bool> DeletePackageAsync(string packageId);
        Task<PackageResponseDto> TogglePackageStatusAsync(string packageId);
        Task<PackageStatsDto> GetPackageStatsAsync();
    }
}
