using GymManagement.Application.DTOs.User.Progress;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.User
{
    public static class ProgressMappings
    {
        public static ProgressResponseDto ToDto(this Progress progress)
        {
            if (progress == null) return null;

            return new ProgressResponseDto
            {
                Id = progress.Id,
                MemberId = progress.MemberId,
                Weight = progress.Weight,
                Height = progress.Height,
                MuscleMass = progress.MuscleMass,
                BodyFat = progress.BodyFat,
                Bmi = progress.BMI,
                CreatedAt = progress.CreatedAt,
                UpdatedAt = progress.UpdatedAt
            };
        }

        public static MonthlyBodyMetricsDto ToMonthlyDto(this Progress progress)
        {
            var month = progress.CreatedAt.Month.ToString("D2");
            var year = progress.CreatedAt.Year;

            return new MonthlyBodyMetricsDto
            {
                Month = $"{month}/{year}",
                Weight = progress.Weight,
                BodyFat = progress.BodyFat,
                MuscleMass = progress.MuscleMass,
                Bmi = progress.BMI
            };
        }

        public static Progress ToEntity(this UpdateBodyMetricsDto dto, string memberId)
        {
            var progress = new Progress
            {
                MemberId = memberId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.Weight.HasValue)
                progress.Weight = dto.Weight.Value;

            if (dto.Height.HasValue)
                progress.Height = dto.Height.Value;

            if (dto.MuscleMass.HasValue)
                progress.MuscleMass = dto.MuscleMass.Value;

            if (dto.BodyFat.HasValue)
                progress.BodyFat = dto.BodyFat.Value;

            // Calculate BMI if both weight and height are provided
            if (dto.Weight.HasValue && dto.Height.HasValue)
            {
                var heightInMeters = dto.Height.Value / 100.0;
                progress.BMI = Math.Round(dto.Weight.Value / (heightInMeters * heightInMeters), 1);
            }

            return progress;
        }
    }
}