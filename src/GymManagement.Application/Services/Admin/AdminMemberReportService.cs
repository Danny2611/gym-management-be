using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using PageSize = PdfSharp.PageSize;

namespace GymManagement.Application.Services.Admin
{
    public class AdminMemberReportService : IAdminMemberReportService
    {
        private readonly IAdminMemberReportRepository _memberReportRepository;

        public AdminMemberReportService(IAdminMemberReportRepository memberReportRepository)
        {
            _memberReportRepository = memberReportRepository;
        }

        public async Task<List<AdminMemberStatsDto>> GetMemberStatsAsync(
            AdminMemberStatsOptions options)
        {
            return await _memberReportRepository.GetMemberStatsAsync(options);
        }

        public async Task<AdminComprehensiveMemberReportDto> GetComprehensiveMemberReportAsync(
            AdminMemberStatsOptions options)
        {
            // Get time series with retention and churn
            var timeSeries = await _memberReportRepository.GetMemberStatsAsync(new AdminMemberStatsOptions
            {
                StartDate = options.StartDate,
                EndDate = options.EndDate,
                GroupBy = options.GroupBy,
                Status = options.Status,
                IncludeRetention = true,
                IncludeChurn = true
            });

            // Get total members count
            var totalMembers = await _memberReportRepository.GetTotalMembersCountAsync(
                new List<string> { "active", "inactive", "pending" });

            // Get status distribution
            var statusCounts = await _memberReportRepository.GetStatusCountsAsync();
            var statusDistribution = new AdminStatusDistributionDto
            {
                Active = new AdminStatusItemDto
                {
                    Count = statusCounts.ContainsKey("active") ? statusCounts["active"] : 0,
                    Percentage = totalMembers > 0
                        ? (decimal)(statusCounts.ContainsKey("active") ? statusCounts["active"] : 0) / totalMembers * 100
                        : 0
                },
                Inactive = new AdminStatusItemDto
                {
                    Count = statusCounts.ContainsKey("inactive") ? statusCounts["inactive"] : 0,
                    Percentage = totalMembers > 0
                        ? (decimal)(statusCounts.ContainsKey("inactive") ? statusCounts["inactive"] : 0) / totalMembers * 100
                        : 0
                },
                Pending = new AdminStatusItemDto
                {
                    Count = statusCounts.ContainsKey("pending") ? statusCounts["pending"] : 0,
                    Percentage = totalMembers > 0
                        ? (decimal)(statusCounts.ContainsKey("pending") ? statusCounts["pending"] : 0) / totalMembers * 100
                        : 0
                },
                Banned = new AdminStatusItemDto
                {
                    Count = statusCounts.ContainsKey("banned") ? statusCounts["banned"] : 0,
                    Percentage = totalMembers > 0
                        ? (decimal)(statusCounts.ContainsKey("banned") ? statusCounts["banned"] : 0) / totalMembers * 100
                        : 0
                }
            };

            // Get retention funnel
            var retentionFunnel = await _memberReportRepository.GetRetentionFunnelAsync();

            // Calculate growth metrics
            var totalGrowth = timeSeries.Sum(t => t.NetGrowth ?? 0);
            var growthRate = timeSeries.Count > 1 && timeSeries[0].TotalMembers > 0
                ? ((decimal)(timeSeries[^1].TotalMembers - timeSeries[0].TotalMembers) / timeSeries[0].TotalMembers) * 100
                : 0;

            var periodOverPeriodGrowth = timeSeries.Count > 1 && timeSeries[^2].NewMembers > 0
                ? ((decimal)(timeSeries[^1].NewMembers - timeSeries[^2].NewMembers) / timeSeries[^2].NewMembers) * 100
                : 0;

            // Top growth periods
            var topGrowthPeriods = timeSeries
                .Where(t => t.NetGrowth.HasValue)
                .OrderByDescending(t => t.NetGrowth!.Value)
                .Take(5)
                .Select(t => new AdminTopGrowthPeriodDto
                {
                    Period = t.Period,
                    Growth = t.NetGrowth!.Value
                })
                .ToList();

            // Churn analysis
            var churnRates = timeSeries.Where(t => t.ChurnRate.HasValue).Select(t => t.ChurnRate!.Value).ToList();
            var averageChurnRate = churnRates.Any() ? churnRates.Average() : 0;

            var highRiskPeriods = timeSeries
                .Where(t => t.ChurnRate.HasValue && t.ChurnRate.Value > averageChurnRate * 1.5m)
                .Select(t => t.Period)
                .ToList();

            // Build retention analysis
            var retentionAnalysis = timeSeries.Select(t => new AdminRetentionAnalysisDto
            {
                Period = t.Period,
                CohortSize = t.NewMembers,
                RetainedMembers = t.RetentionRate.HasValue
                    ? (int)Math.Round((t.RetentionRate.Value * t.NewMembers) / 100)
                    : 0,
                RetentionRate = t.RetentionRate ?? 0,
                ChurnedMembers = t.ExpiredMembers,
                ChurnRate = t.ChurnRate ?? 0
            }).ToList();

            return new AdminComprehensiveMemberReportDto
            {
                Summary = new AdminMemberGrowthAnalysisDto
                {
                    TotalMembers = totalMembers,
                    TotalGrowth = totalGrowth,
                    GrowthRate = Math.Round(growthRate, 2),
                    PeriodOverPeriodGrowth = Math.Round(periodOverPeriodGrowth, 2),
                    RetentionFunnel = retentionFunnel
                },
                TimeSeries = timeSeries,
                StatusDistribution = statusDistribution,
                RetentionAnalysis = retentionAnalysis,
                TopGrowthPeriods = topGrowthPeriods,
                ChurnAnalysis = new AdminChurnAnalysisDto
                {
                    AverageChurnRate = Math.Round(averageChurnRate, 2),
                    HighRiskPeriods = highRiskPeriods,
                    ChurnReasons = new List<AdminChurnReasonDto>() // Would need additional data
                }
            };
        }

        public async Task<byte[]> ExportMemberReportToExcelAsync(AdminMemberStatsOptions options)
        {
            var report = await GetComprehensiveMemberReportAsync(options);

            using var package = new ExcelPackage();

            // Summary Sheet
            var summarySheet = package.Workbook.Worksheets.Add("Summary");
            summarySheet.Cells["A1"].Value = "Member Analytics Summary";
            summarySheet.Cells["A1"].Style.Font.Bold = true;
            summarySheet.Cells["A1"].Style.Font.Size = 16;

            summarySheet.Cells["A3"].Value = "Total Members";
            summarySheet.Cells["B3"].Value = report.Summary.TotalMembers;

            summarySheet.Cells["A4"].Value = "Total Growth";
            summarySheet.Cells["B4"].Value = report.Summary.TotalGrowth;

            summarySheet.Cells["A5"].Value = "Growth Rate (%)";
            summarySheet.Cells["B5"].Value = report.Summary.GrowthRate;
            summarySheet.Cells["B5"].Style.Numberformat.Format = "0.00";

            summarySheet.Cells["A6"].Value = "Period over Period Growth (%)";
            summarySheet.Cells["B6"].Value = report.Summary.PeriodOverPeriodGrowth;
            summarySheet.Cells["B6"].Style.Numberformat.Format = "0.00";

            summarySheet.Cells["A8"].Value = "Retention Funnel";
            summarySheet.Cells["A8"].Style.Font.Bold = true;
            summarySheet.Cells["A9"].Value = "New Members (30 days)";
            summarySheet.Cells["B9"].Value = report.Summary.RetentionFunnel.NewMembers;
            summarySheet.Cells["A10"].Value = "Active after 30+ days";
            summarySheet.Cells["B10"].Value = report.Summary.RetentionFunnel.ActiveAfter30Days;
            summarySheet.Cells["A11"].Value = "Active after 90+ days";
            summarySheet.Cells["B11"].Value = report.Summary.RetentionFunnel.ActiveAfter90Days;
            summarySheet.Cells["A12"].Value = "Active after 180+ days";
            summarySheet.Cells["B12"].Value = report.Summary.RetentionFunnel.ActiveAfter180Days;
            summarySheet.Cells["A13"].Value = "Active after 365+ days";
            summarySheet.Cells["B13"].Value = report.Summary.RetentionFunnel.ActiveAfter365Days;

            summarySheet.Cells[summarySheet.Dimension.Address].AutoFitColumns();

            // Time Series Sheet
            var timeSeriesSheet = package.Workbook.Worksheets.Add("Time Series");
            timeSeriesSheet.Cells["A1"].Value = "Period";
            timeSeriesSheet.Cells["B1"].Value = "Total Members";
            timeSeriesSheet.Cells["C1"].Value = "New Members";
            timeSeriesSheet.Cells["D1"].Value = "Expired Members";
            timeSeriesSheet.Cells["E1"].Value = "Active Members";
            timeSeriesSheet.Cells["F1"].Value = "Inactive Members";
            timeSeriesSheet.Cells["G1"].Value = "Pending Members";
            timeSeriesSheet.Cells["H1"].Value = "Banned Members";
            timeSeriesSheet.Cells["I1"].Value = "Retention Rate (%)";
            timeSeriesSheet.Cells["J1"].Value = "Churn Rate (%)";
            timeSeriesSheet.Cells["K1"].Value = "Growth Rate (%)";
            timeSeriesSheet.Cells["L1"].Value = "Net Growth";
            timeSeriesSheet.Cells["A1:L1"].Style.Font.Bold = true;

            int row = 2;
            foreach (var item in report.TimeSeries)
            {
                timeSeriesSheet.Cells[row, 1].Value = item.Period;
                timeSeriesSheet.Cells[row, 2].Value = item.TotalMembers;
                timeSeriesSheet.Cells[row, 3].Value = item.NewMembers;
                timeSeriesSheet.Cells[row, 4].Value = item.ExpiredMembers;
                timeSeriesSheet.Cells[row, 5].Value = item.ActiveMembers;
                timeSeriesSheet.Cells[row, 6].Value = item.InactiveMembers;
                timeSeriesSheet.Cells[row, 7].Value = item.PendingMembers;
                timeSeriesSheet.Cells[row, 8].Value = item.BannedMembers;
                timeSeriesSheet.Cells[row, 9].Value = item.RetentionRate.HasValue ? (double)item.RetentionRate.Value : 0;
                timeSeriesSheet.Cells[row, 9].Style.Numberformat.Format = "0.00";
                timeSeriesSheet.Cells[row, 10].Value = item.ChurnRate.HasValue ? (double)item.ChurnRate.Value : 0;
                timeSeriesSheet.Cells[row, 10].Style.Numberformat.Format = "0.00";
                timeSeriesSheet.Cells[row, 11].Value = item.GrowthRate.HasValue ? (double)item.GrowthRate.Value : 0;
                timeSeriesSheet.Cells[row, 11].Style.Numberformat.Format = "0.00";
                timeSeriesSheet.Cells[row, 12].Value = item.NetGrowth ?? 0;
                row++;
            }

            timeSeriesSheet.Cells[timeSeriesSheet.Dimension.Address].AutoFitColumns();

            // Status Distribution Sheet
            var statusSheet = package.Workbook.Worksheets.Add("Status Distribution");
            statusSheet.Cells["A1"].Value = "Status";
            statusSheet.Cells["B1"].Value = "Count";
            statusSheet.Cells["C1"].Value = "Percentage";
            statusSheet.Cells["A1:C1"].Style.Font.Bold = true;

            statusSheet.Cells["A2"].Value = "Active";
            statusSheet.Cells["B2"].Value = report.StatusDistribution.Active.Count;
            statusSheet.Cells["C2"].Value = (double)report.StatusDistribution.Active.Percentage;
            statusSheet.Cells["C2"].Style.Numberformat.Format = "0.00\"%\"";

            statusSheet.Cells["A3"].Value = "Inactive";
            statusSheet.Cells["B3"].Value = report.StatusDistribution.Inactive.Count;
            statusSheet.Cells["C3"].Value = (double)report.StatusDistribution.Inactive.Percentage;
            statusSheet.Cells["C3"].Style.Numberformat.Format = "0.00\"%\"";

            statusSheet.Cells["A4"].Value = "Pending";
            statusSheet.Cells["B4"].Value = report.StatusDistribution.Pending.Count;
            statusSheet.Cells["C4"].Value = (double)report.StatusDistribution.Pending.Percentage;
            statusSheet.Cells["C4"].Style.Numberformat.Format = "0.00\"%\"";

            statusSheet.Cells["A5"].Value = "Banned";
            statusSheet.Cells["B5"].Value = report.StatusDistribution.Banned.Count;
            statusSheet.Cells["C5"].Value = (double)report.StatusDistribution.Banned.Percentage;
            statusSheet.Cells["C5"].Style.Numberformat.Format = "0.00\"%\"";

            statusSheet.Cells[statusSheet.Dimension.Address].AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportMemberReportToPdfAsync(AdminMemberStatsOptions options)
        {
            var report = await GetComprehensiveMemberReportAsync(options);

            var html = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 40px; }}
                        h1 {{ color: #333; font-size: 24px; }}
                        h2 {{ color: #666; font-size: 18px; margin-top: 30px; }}
                        table {{ width: 100%; border-collapse: collapse; margin-top: 15px; }}
                        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                        th {{ background-color: #f2f2f2; font-weight: bold; }}
                        .summary {{ margin-bottom: 20px; }}
                        .summary-item {{ margin: 10px 0; }}
                    </style>
                </head>
                <body>
                    <h1>Member Analytics Report</h1>
                    
                    <h2>Summary</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>Total Members:</strong> {report.Summary.TotalMembers}</div>
                        <div class='summary-item'><strong>Total Growth:</strong> {report.Summary.TotalGrowth}</div>
                        <div class='summary-item'><strong>Growth Rate:</strong> {report.Summary.GrowthRate:N2}%</div>
                        <div class='summary-item'><strong>Period over Period Growth:</strong> {report.Summary.PeriodOverPeriodGrowth:N2}%</div>
                    </div>

                    <h2>Retention Funnel</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>New Members (30 days):</strong> {report.Summary.RetentionFunnel.NewMembers}</div>
                        <div class='summary-item'><strong>Active after 30+ days:</strong> {report.Summary.RetentionFunnel.ActiveAfter30Days}</div>
                        <div class='summary-item'><strong>Active after 90+ days:</strong> {report.Summary.RetentionFunnel.ActiveAfter90Days}</div>
                        <div class='summary-item'><strong>Active after 180+ days:</strong> {report.Summary.RetentionFunnel.ActiveAfter180Days}</div>
                        <div class='summary-item'><strong>Active after 365+ days:</strong> {report.Summary.RetentionFunnel.ActiveAfter365Days}</div>
                    </div>

                    <h2>Status Distribution</h2>
                    <table>
                        <tr>
                            <th>Status</th>
                            <th>Count</th>
                            <th>Percentage</th>
                        </tr>
                        <tr>
                            <td>Active</td>
                            <td>{report.StatusDistribution.Active.Count}</td>
                            <td>{report.StatusDistribution.Active.Percentage:N1}%</td>
                        </tr>
                        <tr>
                            <td>Inactive</td>
                            <td>{report.StatusDistribution.Inactive.Count}</td>
                            <td>{report.StatusDistribution.Inactive.Percentage:N1}%</td>
                        </tr>
                        <tr>
                            <td>Pending</td>
                            <td>{report.StatusDistribution.Pending.Count}</td>
                            <td>{report.StatusDistribution.Pending.Percentage:N1}%</td>
                        </tr>
                        <tr>
                            <td>Banned</td>
                            <td>{report.StatusDistribution.Banned.Count}</td>
                            <td>{report.StatusDistribution.Banned.Percentage:N1}%</td>
                        </tr>
                    </table>

                    <h2>Churn Analysis</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>Average Churn Rate:</strong> {report.ChurnAnalysis.AverageChurnRate:N2}%</div>
                        <div class='summary-item'><strong>High Risk Periods:</strong> {string.Join(", ", report.ChurnAnalysis.HighRiskPeriods)}</div>
                    </div>
                </body>
                </html>";

            var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            using var stream = new MemoryStream();
            pdf.Save(stream, false);
            return stream.ToArray();
        }
    }
}