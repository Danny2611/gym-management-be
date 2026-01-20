using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminDashboardReportRepository
    {
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync(AdminReportDateRange dateRange);
        Task<AdminAdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(AdminReportDateRange dateRange);
    }
}