using GymManagement.Application.DTOs.User.Progress;
using GymManagement.Domain.Entities;
namespace GymManagement.Application.Services.User
{
    public interface IProgressService
    {
        Task<Progress> GetLatestBodyMetricsAsync(string memberId);
        Task<Progress> GetInitialBodyMetricsAsync(string memberId);
        Task<Progress> GetPreviousMonthBodyMetricsAsync(string memberId);
        Task<BodyMetricsComparisonDto> GetBodyMetricsComparisonAsync(string memberId);
        Task<Progress> UpdateBodyMetricsAsync(UpdateBodyMetricsDto dto, string memberId);
        Task<List<MonthlyBodyMetricsDto>> GetBodyStatsProgressByMonthAsync(string memberId, int months);
        Task<List<FitnessRadarDto>> GetFitnessRadarDataAsync(string memberId);
        Task<BodyMetricsChangeResponseDto> CalculateBodyMetricsChangeAsync(string memberId);
        Task<List<MonthlyBodyMetricsDto>> GetFormattedMonthlyBodyMetricsAsync(string memberId);
    }
}