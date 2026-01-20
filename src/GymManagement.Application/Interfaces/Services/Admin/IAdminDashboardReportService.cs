using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminDashboardReportService
    {
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync(AdminReportDateRange dateRange);
        Task<AdminAdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(AdminReportDateRange dateRange);
    }
}