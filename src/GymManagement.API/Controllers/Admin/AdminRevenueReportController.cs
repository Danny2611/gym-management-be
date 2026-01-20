using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/reports/revenue")]
    public class RevenueReportController : ControllerBase
    {
        private readonly IAdminRevenueReportService _revenueReportService;
        private readonly ILogger<RevenueReportController> _logger;

        public RevenueReportController(
            IAdminRevenueReportService revenueReportService,
            ILogger<RevenueReportController> logger)
        {
            _revenueReportService = revenueReportService;
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

        // Helper method to validate groupBy parameter
        private string ValidateGroupBy(string? groupBy)
        {
            var validGroupBy = new[] { "day", "week", "month", "year" };
            return !string.IsNullOrEmpty(groupBy) && validGroupBy.Contains(groupBy.ToLower())
                ? groupBy.ToLower()
                : "month";
        }

        // Helper method to validate category parameter
        private string? ValidateCategory(string? category)
        {
            var validCategories = new[] { "basic", "fitness", "premium", "platinum", "vip" };
            return !string.IsNullOrEmpty(category) && validCategories.Contains(category.ToLower())
                ? category.ToLower()
                : null;
        }

        // GET: api/admin/reports/revenue/packages
        [HttpGet("packages")]
        public async Task<IActionResult> GetRevenueByPackages(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? packageId = null,
            [FromQuery] string? category = null,
            [FromQuery] string? groupBy = null)
        {
            try
            {
                var options = new RevenueReportOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    PackageId = packageId,
                    Category = ValidateCategory(category),
                    GroupBy = ValidateGroupBy(groupBy)
                };

                var revenueData = await _revenueReportService.GetRevenueByPackagesAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Revenue by packages retrieved successfully",
                    data = revenueData,
                    pagination = new
                    {
                        total = revenueData.Count,
                        page = 1,
                        limit = revenueData.Count
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRevenueByPackages");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while retrieving revenue by packages",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/time-series
        [HttpGet("time-series")]
        public async Task<IActionResult> GetRevenueTimeSeries(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = null,
            [FromQuery] string? packageId = null,
            [FromQuery] string? category = null)
        {
            try
            {
                var options = new RevenueReportOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    PackageId = packageId,
                    Category = ValidateCategory(category)
                };

                var timeSeriesData = await _revenueReportService.GetRevenueTimeSeriesAsync(options);

                var totalRevenue = timeSeriesData.Sum(item => item.TotalRevenue);
                var totalSales = timeSeriesData.Sum(item => item.TotalSales);

                return Ok(new
                {
                    success = true,
                    message = "Revenue time series data retrieved successfully",
                    data = timeSeriesData,
                    summary = new
                    {
                        totalPeriods = timeSeriesData.Count,
                        totalRevenue,
                        totalSales,
                        groupBy = options.GroupBy
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRevenueTimeSeries");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while retrieving revenue time series",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/analytics
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAdvancedAnalytics(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null)
        {
            try
            {
                var parsedStartDate = ParseDate(startDate);
                var parsedEndDate = ParseDate(endDate);

                var analyticsData = await _revenueReportService.GetAdvancedAnalyticsAsync(
                    parsedStartDate,
                    parsedEndDate
                );

                return Ok(new
                {
                    success = true,
                    message = "Advanced analytics retrieved successfully",
                    data = analyticsData,
                    generatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAdvancedAnalytics");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while retrieving advanced analytics",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/comprehensive
        [HttpGet("comprehensive")]
        public async Task<IActionResult> GetComprehensiveRevenueReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = null,
            [FromQuery] string? packageId = null,
            [FromQuery] string? category = null)
        {
            try
            {
                var options = new RevenueReportOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    PackageId = packageId,
                    Category = ValidateCategory(category)
                };

                var comprehensiveReport = await _revenueReportService
                    .GetComprehensiveRevenueReportAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Comprehensive revenue report retrieved successfully",
                    data = comprehensiveReport,
                    metadata = new
                    {
                        generatedAt = DateTime.UtcNow,
                        dateRange = new
                        {
                            startDate = options.StartDate?.ToString("O"),
                            endDate = options.EndDate?.ToString("O")
                        },
                        filters = new
                        {
                            groupBy = options.GroupBy,
                            packageId = options.PackageId,
                            category = options.Category
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetComprehensiveRevenueReport");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while retrieving comprehensive revenue report",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/export/excel
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportRevenueReportToExcel(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = null,
            [FromQuery] string? packageId = null,
            [FromQuery] string? category = null)
        {
            try
            {
                var options = new RevenueReportOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    PackageId = packageId,
                    Category = ValidateCategory(category)
                };

                var excelBytes = await _revenueReportService.ExportRevenueReportToExcelAsync(options);

                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
                var filename = $"revenue-report-{timestamp}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportRevenueReportToExcel");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while exporting revenue report to Excel",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/export/pdf
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportRevenueReportToPdf(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = null,
            [FromQuery] string? packageId = null,
            [FromQuery] string? category = null)
        {
            try
            {
                var options = new RevenueReportOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    PackageId = packageId,
                    Category = ValidateCategory(category)
                };

                var pdfBytes = await _revenueReportService.ExportRevenueReportToPdfAsync(options);

                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
                var filename = $"revenue-report-{timestamp}.pdf";

                return File(
                    pdfBytes,
                    "application/pdf",
                    filename
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportRevenueReportToPdf");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while exporting revenue report to PDF",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetRevenueDashboard(
            [FromQuery] int period = 30)
        {
            try
            {
                // Validate period (must be positive)
                if (period <= 0)
                {
                    period = 30;
                }

                var dashboardData = await _revenueReportService.GetRevenueDashboardAsync(period);

                return Ok(new
                {
                    success = true,
                    message = "Revenue dashboard data retrieved successfully",
                    data = dashboardData,
                    generatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRevenueDashboard");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while retrieving revenue dashboard",
                    error = ex.Message
                });
            }
        }
    }
}