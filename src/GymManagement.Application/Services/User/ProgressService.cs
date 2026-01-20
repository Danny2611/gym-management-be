using GymManagement.Application.DTOs.User.Progress;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Application.Mappings.User;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{

    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _repository;

        public ProgressService(IProgressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Progress> GetLatestBodyMetricsAsync(string memberId)
        {
            return await _repository.GetLatestAsync(memberId);
        }

        public async Task<Progress> GetInitialBodyMetricsAsync(string memberId)
        {
            return await _repository.GetInitialAsync(memberId);
        }

        public async Task<Progress> GetPreviousMonthBodyMetricsAsync(string memberId)
        {
            return await _repository.GetPreviousMonthAsync(memberId);
        }

        public async Task<BodyMetricsComparisonDto> GetBodyMetricsComparisonAsync(string memberId)
        {
            var current = await _repository.GetLatestAsync(memberId);
            var initial = await _repository.GetInitialAsync(memberId);
            var previous = await _repository.GetPreviousMonthAsync(memberId);

            return new BodyMetricsComparisonDto
            {
                Current = current?.ToDto(),
                Initial = initial?.ToDto(),
                Previous = previous?.ToDto()
            };
        }

        public async Task<Progress> UpdateBodyMetricsAsync(UpdateBodyMetricsDto dto, string memberId)
        {
            // Validate that at least one metric is provided
            if (!dto.Weight.HasValue && !dto.Height.HasValue &&
                !dto.MuscleMass.HasValue && !dto.BodyFat.HasValue)
            {
                throw new ArgumentException("Cần cung cấp ít nhất một chỉ số cơ thể để cập nhật");
            }

            // Get latest metrics to fill in missing values
            var latest = await _repository.GetLatestAsync(memberId);

            var progress = new Progress
            {
                MemberId = memberId,
                Weight = dto.Weight ?? latest?.Weight ?? 0,
                Height = dto.Height ?? latest?.Height ?? 0,
                MuscleMass = dto.MuscleMass ?? latest?.MuscleMass ?? 0,
                BodyFat = dto.BodyFat ?? latest?.BodyFat ?? 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Calculate BMI
            if (progress.Weight > 0 && progress.Height > 0)
            {
                var heightInMeters = progress.Height / 100.0;
                progress.BMI = Math.Round(progress.Weight / (heightInMeters * heightInMeters), 1);
            }

            return await _repository.CreateAsync(progress);
        }

        public async Task<List<MonthlyBodyMetricsDto>> GetBodyStatsProgressByMonthAsync(
            string memberId,
            int months)
        {
            var startDate = DateTime.UtcNow.AddMonths(-months + 1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);

            var records = await _repository.GetByDateRangeAsync(memberId, startDate, DateTime.UtcNow);

            // Group by month and get the last record for each month
            var monthlyRecords = records
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => g.OrderByDescending(r => r.CreatedAt).First())
                .OrderBy(r => r.CreatedAt)
                .ToList();

            return monthlyRecords.Select(r => new MonthlyBodyMetricsDto
            {
                Month = $"{r.CreatedAt.Month}/{r.CreatedAt.Year}",
                Weight = r.Weight,
                BodyFat = r.BodyFat,
                MuscleMass = r.MuscleMass,
                Bmi = r.BMI
            }).ToList();
        }

        public async Task<List<FitnessRadarDto>> GetFitnessRadarDataAsync(string memberId)
        {
            var current = await _repository.GetLatestAsync(memberId);
            var initial = await _repository.GetInitialAsync(memberId);

            if (current == null || initial == null)
            {
                throw new InvalidOperationException("Không có đủ dữ liệu để tạo biểu đồ radar");
            }

            // Ideal values for calculation
            const double IDEAL_BODY_FAT = 15.0;
            const double IDEAL_MUSCLE_MASS = 40.0;
            const double IDEAL_BMI = 22.0;

            var radarData = new List<FitnessRadarDto>
            {
                new FitnessRadarDto
                {
                    Subject = "Sức bền",
                    Current = Math.Round(CalculateEnduranceScore(current.BodyFat), 1),
                    Initial = Math.Round(CalculateEnduranceScore(initial.BodyFat), 1),
                    Full = 10
                },
                new FitnessRadarDto
                {
                    Subject = "Sức mạnh",
                    Current = Math.Round(CalculateStrengthScore(current.MuscleMass), 1),
                    Initial = Math.Round(CalculateStrengthScore(initial.MuscleMass), 1),
                    Full = 10
                },
                new FitnessRadarDto
                {
                    Subject = "Linh hoạt",
                    Current = Math.Round(CalculateFlexibilityScore(current.Weight, current.Height), 1),
                    Initial = Math.Round(CalculateFlexibilityScore(initial.Weight, initial.Height), 1),
                    Full = 10
                },
                new FitnessRadarDto
                {
                    Subject = "Cân đối",
                    Current = Math.Round(CalculateBalanceScore(current.BMI), 1),
                    Initial = Math.Round(CalculateBalanceScore(initial.BMI), 1),
                    Full = 10
                },
                new FitnessRadarDto
                {
                    Subject = "Tim mạch",
                    Current = Math.Round(CalculateCardioScore(current.BodyFat, current.MuscleMass), 1),
                    Initial = Math.Round(CalculateCardioScore(initial.BodyFat, initial.MuscleMass), 1),
                    Full = 10
                }
            };

            return radarData;
        }

        public async Task<BodyMetricsChangeResponseDto> CalculateBodyMetricsChangeAsync(string memberId)
        {
            var comparison = await GetBodyMetricsComparisonAsync(memberId);

            if (comparison.Current == null || comparison.Initial == null)
            {
                throw new InvalidOperationException("Không có đủ dữ liệu để tính toán thay đổi");
            }

            var changes = new MetricsChangeDto
            {
                Weight = CalculatePercentChange(comparison.Current.Weight, comparison.Initial.Weight),
                BodyFat = CalculatePercentChange(comparison.Current.BodyFat, comparison.Initial.BodyFat),
                MuscleMass = CalculatePercentChange(comparison.Current.MuscleMass, comparison.Initial.MuscleMass),
                Bmi = CalculatePercentChange(comparison.Current.Bmi, comparison.Initial.Bmi)
            };

            return new BodyMetricsChangeResponseDto
            {
                Changes = changes,
                Current = comparison.Current,
                Initial = comparison.Initial
            };
        }

        public async Task<List<MonthlyBodyMetricsDto>> GetFormattedMonthlyBodyMetricsAsync(string memberId)
        {
            var monthlyMetrics = await _repository.GetMonthlyMetricsAsync(memberId);
            return monthlyMetrics.Select(m => m.ToMonthlyDto()).ToList();
        }

        // Helper methods for fitness radar calculations
        private double CalculateStrengthScore(double muscleMass)
        {
            const double IDEAL_MUSCLE_MASS = 40.0;
            return Math.Min(10, Math.Max(1, (muscleMass / IDEAL_MUSCLE_MASS) * 10));
        }

        private double CalculateEnduranceScore(double bodyFat)
        {
            const double IDEAL_BODY_FAT = 15.0;
            var idealDiff = Math.Abs(bodyFat - IDEAL_BODY_FAT);
            return Math.Min(10, Math.Max(1, 10 - idealDiff / 2));
        }

        private double CalculateBalanceScore(double bmi)
        {
            const double IDEAL_BMI = 22.0;
            var idealDiff = Math.Abs(bmi - IDEAL_BMI);
            return Math.Min(10, Math.Max(1, 10 - idealDiff));
        }

        private double CalculateFlexibilityScore(double weight, double height)
        {
            var ratio = weight / height;
            const double IDEAL_RATIO = 0.4;
            var diff = Math.Abs(ratio - IDEAL_RATIO);
            return Math.Min(10, Math.Max(1, 10 - diff * 20));
        }

        private double CalculateCardioScore(double bodyFat, double muscleMass)
        {
            if (bodyFat == 0) return 1;
            var ratio = muscleMass / bodyFat;
            return Math.Min(10, Math.Max(1, ratio * 2));
        }

        private string CalculatePercentChange(double current, double previous)
        {
            if (previous == 0) return "0";
            var change = ((current - previous) / previous) * 100;
            return change.ToString("F1");
        }
    }
}