using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/reports")]
    public class AdminDashboardReportController : ControllerBase
    {
        private readonly IAdminDashboardReportService _dashboardReportService;
        private readonly ILogger<AdminDashboardReportController> _logger;

        public AdminDashboardReportController(
            IAdminDashboardReportService dashboardReportService,
            ILogger<AdminDashboardReportController> logger)
        {
            _dashboardReportService = dashboardReportService;
            _logger = logger;
        }

        // Helper method to validate and parse date
        private DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            if (DateTime.TryParse(dateString, out var date))
                return date;

            return null;
        }

        // GET: api/admin/reports/dashboard/stats
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var dateRange = new AdminReportDateRange
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate)
                };

                var dashboardData = await _dashboardReportService.GetDashboardStatsAsync(dateRange);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê dashboard thành công",
                    data = dashboardData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê dashboard");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê dashboard"
                });
            }
        }

        // GET: api/admin/reports/advanced-analytics
        [HttpGet("advanced-analytics")]
        public async Task<IActionResult> GetAdvancedAnalytics(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var dateRange = new AdminReportDateRange
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate)
                };

                var analyticsData = await _dashboardReportService.GetAdvancedAnalyticsAsync(dateRange);

                return Ok(new
                {
                    success = true,
                    message = "Lấy phân tích nâng cao thành công",
                    data = analyticsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phân tích nâng cao");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy phân tích nâng cao"
                });
            }
        }
    }
}