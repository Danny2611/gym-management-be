using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.Application.Services.Admin
{
    public class AdminDashboardReportService : IAdminDashboardReportService
    {
        private readonly IAdminDashboardReportRepository _dashboardReportRepository;

        public AdminDashboardReportService(IAdminDashboardReportRepository dashboardReportRepository)
        {
            _dashboardReportRepository = dashboardReportRepository;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync(
            AdminReportDateRange dateRange)
        {
            return await _dashboardReportRepository.GetDashboardStatsAsync(dateRange);
        }

        public async Task<AdminAdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(
            AdminReportDateRange dateRange)
        {
            return await _dashboardReportRepository.GetAdvancedAnalyticsAsync(dateRange);
        }
    }
}