using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminRevenueReportService
    {
        Task<List<RevenueByPackageDto>> GetRevenueByPackagesAsync(RevenueReportOptions options);
        Task<List<RevenueTimeSeriesDto>> GetRevenueTimeSeriesAsync(RevenueReportOptions options);
        Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(DateTime? startDate, DateTime? endDate);
        Task<ComprehensiveRevenueReportDto> GetComprehensiveRevenueReportAsync(RevenueReportOptions options);
        Task<byte[]> ExportRevenueReportToExcelAsync(RevenueReportOptions options);
        Task<byte[]> ExportRevenueReportToPdfAsync(RevenueReportOptions options);
        Task<RevenueDashboardDto> GetRevenueDashboardAsync(int period);
    }
}