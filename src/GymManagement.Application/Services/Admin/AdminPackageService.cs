using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using GymManagement.Application.Mappings;

namespace GymManagement.Application.Services.Admin
{
    public class AdminPackageService : IAdminPackageService
    {
        private readonly IAdminPackageRepository _packageRepository;

        public AdminPackageService(IAdminPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<PackageListResponseDto> GetAllPackagesAsync(PackageQueryOptions options)
        {
            var (packages, totalCount) = await _packageRepository.GetAllAsync(options);

            var totalPages = (int)Math.Ceiling((double)totalCount / options.Limit);

            // Get package details for all packages
            var packageDtos = new List<PackageResponseDto>();
            foreach (var package in packages)
            {
                var detail = await _packageRepository.GetDetailByPackageIdAsync(package.Id);
                packageDtos.Add(package.ToDto(detail));
            }

            return new PackageListResponseDto
            {
                Packages = packageDtos,
                TotalPackages = totalCount,
                TotalPages = totalPages,
                CurrentPage = options.Page
            };
        }

        public async Task<PackageResponseDto> GetPackageByIdAsync(string packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);

            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói dịch vụ");
            }

            var detail = await _packageRepository.GetDetailByPackageIdAsync(packageId);

            return package.ToDto(detail);
        }

        public async Task<PackageResponseDto> CreatePackageAsync(CreatePackageDto dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || dto.Price <= 0 || dto.Duration <= 0)
            {
                throw new ArgumentException("Thiếu thông tin bắt buộc của gói dịch vụ");
            }

            // Create package
            var package = dto.ToEntity();
            var createdPackage = await _packageRepository.CreateAsync(package);

            // Create package detail if provided
            Domain.Entities.PackageDetail createdDetail = null;
            if (dto.PackageDetail != null)
            {
                var detail = dto.PackageDetail.ToDetailEntity(createdPackage.Id);
                createdDetail = await _packageRepository.CreateDetailAsync(detail);
            }

            return createdPackage.ToDto(createdDetail);
        }

        public async Task<PackageResponseDto> UpdatePackageAsync(string packageId, UpdatePackageDto dto)
        {
            var existingPackage = await _packageRepository.GetByIdAsync(packageId);

            if (existingPackage == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói dịch vụ");
            }

            // Update package fields
            if (!string.IsNullOrEmpty(dto.Name))
                existingPackage.Name = dto.Name;

            if (dto.MaxMembers.HasValue)
                existingPackage.MaxMembers = dto.MaxMembers ?? 0;

            if (dto.Price.HasValue)
                existingPackage.Price = dto.Price.Value;

            if (dto.Duration.HasValue)
                existingPackage.Duration = dto.Duration.Value;

            if (!string.IsNullOrEmpty(dto.Description))
                existingPackage.Description = dto.Description;

            if (dto.Benefits != null)
                existingPackage.Benefits = dto.Benefits;

            if (!string.IsNullOrEmpty(dto.Status))
                existingPackage.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Category))
                existingPackage.Category = dto.Category;

            if (dto.Popular.HasValue)
                existingPackage.Popular = dto.Popular.Value;

            if (dto.TrainingSessions.HasValue)
                existingPackage.TrainingSessions = dto.TrainingSessions.Value;

            if (dto.SessionDuration.HasValue)
                existingPackage.SessionDuration = dto.SessionDuration.Value;

            var updatedPackage = await _packageRepository.UpdateAsync(packageId, existingPackage);

            // Update or create package detail if provided
            Domain.Entities.PackageDetail updatedDetail = null;
            if (dto.PackageDetail != null)
            {
                var detail = dto.PackageDetail.ToDetailEntity(packageId);
                updatedDetail = await _packageRepository.UpdateDetailAsync(packageId, detail);
            }
            else
            {
                updatedDetail = await _packageRepository.GetDetailByPackageIdAsync(packageId);
            }

            return updatedPackage.ToDto(updatedDetail);
        }

        public async Task<bool> DeletePackageAsync(string packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);

            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói dịch vụ");
            }

            return await _packageRepository.SoftDeleteAsync(packageId);
        }

        public async Task<PackageResponseDto> TogglePackageStatusAsync(string packageId)
        {
            var updatedPackage = await _packageRepository.ToggleStatusAsync(packageId);
            var detail = await _packageRepository.GetDetailByPackageIdAsync(packageId);

            return updatedPackage.ToDto(detail);
        }

        public async Task<PackageStatsDto> GetPackageStatsAsync()
        {
            return await _packageRepository.GetStatsAsync();
        }
    }
}