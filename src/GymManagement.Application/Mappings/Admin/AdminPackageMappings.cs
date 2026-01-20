using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings
{
    public static class AdminPackageMappings
    {
        public static PackageResponseDto ToDto(
            this Package package,
            PackageDetail packageDetail = null)
        {
            return new PackageResponseDto
            {
                Id = package.Id,
                Name = package.Name,
                MaxMembers = package.MaxMembers,
                Price = package.Price,
                Duration = package.Duration,
                Description = package.Description,
                Benefits = package.Benefits ?? new List<string>(),
                Status = package.Status,
                Category = package.Category,
                Popular = package.Popular,
                TrainingSessions = package.TrainingSessions,
                SessionDuration = package.SessionDuration,
                CreatedAt = package.CreatedAt,
                UpdatedAt = package.UpdatedAt,
                PackageDetail = packageDetail != null ? new PackageDetailDto
                {
                    Schedule = packageDetail.Schedule ?? new List<string>(),
                    TrainingAreas = packageDetail.TrainingAreas ?? new List<string>(),
                    AdditionalServices = packageDetail.AdditionalServices ?? new List<string>()
                } : null
            };
        }

        public static Package ToEntity(this CreatePackageDto dto)
        {
            return new Package
            {
                Name = dto.Name,
                MaxMembers = dto.MaxMembers ?? 0,
                Price = dto.Price,
                Duration = dto.Duration,
                Description = dto.Description,
                Benefits = dto.Benefits ?? new List<string>(),
                Status = dto.Status ?? "active",
                Category = dto.Category ?? "basic",
                Popular = dto.Popular ?? false,
                TrainingSessions = dto.TrainingSessions ?? 0,
                SessionDuration = dto.SessionDuration ?? 60,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static PackageDetail ToDetailEntity(this PackageDetailInputDto dto, string packageId)
        {
            return new PackageDetail
            {
                PackageId = packageId,
                Schedule = dto?.Schedule ?? new List<string>(),
                TrainingAreas = dto?.TrainingAreas ?? new List<string>(),
                AdditionalServices = dto?.AdditionalServices ?? new List<string>(),
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}