using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Services.Admin;

namespace GymManagement.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/reports/members")]
    public class AdminMemberReportController : ControllerBase
    {
        private readonly IAdminMemberReportService _memberReportService;
        private readonly ILogger<AdminMemberReportController> _logger;

        public AdminMemberReportController(
            IAdminMemberReportService memberReportService,
            ILogger<AdminMemberReportController> logger)
        {
            _memberReportService = memberReportService;
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

        // Helper method to validate status parameter
        private string? ValidateStatus(string? status)
        {
            var validStatuses = new[] { "active", "inactive", "pending", "banned" };
            return !string.IsNullOrEmpty(status) && validStatuses.Contains(status.ToLower())
                ? status.ToLower()
                : null;
        }

        // GET: api/admin/reports/members/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetMemberStats(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = "month",
            [FromQuery] string? status = null,
            [FromQuery] bool includeRetention = false,
            [FromQuery] bool includeChurn = false)
        {
            try
            {
                var parsedStartDate = ParseDate(startDate);
                var parsedEndDate = ParseDate(endDate);

                // Validate date range
                if (parsedStartDate.HasValue && parsedEndDate.HasValue &&
                    parsedStartDate.Value > parsedEndDate.Value)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ngày bắt đầu không thể lớn hơn ngày kết thúc"
                    });
                }

                var options = new AdminMemberStatsOptions
                {
                    StartDate = parsedStartDate,
                    EndDate = parsedEndDate,
                    GroupBy = ValidateGroupBy(groupBy),
                    Status = ValidateStatus(status),
                    IncludeRetention = includeRetention,
                    IncludeChurn = includeChurn
                };

                var stats = await _memberReportService.GetMemberStatsAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy thống kê thành viên thành công",
                    data = stats,
                    meta = new
                    {
                        total = stats.Count,
                        groupBy = options.GroupBy,
                        dateRange = new
                        {
                            startDate = options.StartDate,
                            endDate = options.EndDate
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy thống kê thành viên"
                });
            }
        }

        // GET: api/admin/reports/members/comprehensive
        [HttpGet("comprehensive")]
        public async Task<IActionResult> GetComprehensiveMemberReport(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = "month",
            [FromQuery] string? status = null,
            [FromQuery] bool cohortAnalysis = false)
        {
            try
            {
                var parsedStartDate = ParseDate(startDate);
                var parsedEndDate = ParseDate(endDate);

                // Validate date range
                if (parsedStartDate.HasValue && parsedEndDate.HasValue &&
                    parsedStartDate.Value > parsedEndDate.Value)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ngày bắt đầu không thể lớn hơn ngày kết thúc"
                    });
                }

                var options = new AdminMemberStatsOptions
                {
                    StartDate = parsedStartDate,
                    EndDate = parsedEndDate,
                    GroupBy = ValidateGroupBy(groupBy),
                    Status = ValidateStatus(status),
                    CohortAnalysis = cohortAnalysis
                };

                var report = await _memberReportService.GetComprehensiveMemberReportAsync(options);

                return Ok(new
                {
                    success = true,
                    message = "Lấy báo cáo tổng hợp thành viên thành công",
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy báo cáo tổng hợp thành viên");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi lấy báo cáo tổng hợp thành viên"
                });
            }
        }

        // GET: api/admin/reports/members/export/excel
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportMemberReportToExcel(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = "month",
            [FromQuery] string? status = null)
        {
            try
            {
                var options = new AdminMemberStatsOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    Status = ValidateStatus(status)
                };

                var excelBytes = await _memberReportService.ExportMemberReportToExcelAsync(options);

                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
                var filename = $"member-report-{timestamp}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất báo cáo Excel");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xuất báo cáo Excel"
                });
            }
        }

        // GET: api/admin/reports/members/export/pdf
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportMemberReportToPdf(
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? groupBy = "month",
            [FromQuery] string? status = null)
        {
            try
            {
                var options = new AdminMemberStatsOptions
                {
                    StartDate = ParseDate(startDate),
                    EndDate = ParseDate(endDate),
                    GroupBy = ValidateGroupBy(groupBy),
                    Status = ValidateStatus(status)
                };

                var pdfBytes = await _memberReportService.ExportMemberReportToPdfAsync(options);

                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss");
                var filename = $"member-report-{timestamp}.pdf";

                return File(
                    pdfBytes,
                    "application/pdf",
                    filename
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất báo cáo PDF");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi server khi xuất báo cáo PDF"
                });
            }
        }
    }
}