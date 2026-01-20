using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminRevenueReportRepository
    {
        Task<List<RevenueByPackageDto>> GetRevenueByPackagesAsync(RevenueReportOptions options);
        Task<List<RevenueTimeSeriesDto>> GetRevenueTimeSeriesAsync(RevenueReportOptions options);
        Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(DateTime? startDate, DateTime? endDate);
    }
}