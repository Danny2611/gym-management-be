using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Services.Admin;
using OfficeOpenXml;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PageSize = PdfSharp.PageSize;
namespace GymManagement.Application.Services.Admin
{
    public class AdminRevenueReportService : IAdminRevenueReportService
    {
        private readonly IAdminRevenueReportRepository _revenueReportRepository;

        public AdminRevenueReportService(IAdminRevenueReportRepository revenueReportRepository)
        {
            _revenueReportRepository = revenueReportRepository;


        }

        public async Task<List<RevenueByPackageDto>> GetRevenueByPackagesAsync(
            RevenueReportOptions options)
        {
            return await _revenueReportRepository.GetRevenueByPackagesAsync(options);
        }

        public async Task<List<RevenueTimeSeriesDto>> GetRevenueTimeSeriesAsync(
            RevenueReportOptions options)
        {
            return await _revenueReportRepository.GetRevenueTimeSeriesAsync(options);
        }

        public async Task<AdvancedAnalyticsDto> GetAdvancedAnalyticsAsync(
            DateTime? startDate,
            DateTime? endDate)
        {
            return await _revenueReportRepository.GetAdvancedAnalyticsAsync(startDate, endDate);
        }



        public async Task<ComprehensiveRevenueReportDto> GetComprehensiveRevenueReportAsync(
    RevenueReportOptions options)
        {
            var revenueByPackages = await GetRevenueByPackagesAsync(options);
            var timeSeries = await GetRevenueTimeSeriesAsync(options);
            var analytics = await GetAdvancedAnalyticsAsync(options.StartDate, options.EndDate);

            // Calculate summary statistics - handle empty data
            var totalRevenue = revenueByPackages.Sum(p => p.TotalRevenue);
            var totalSales = revenueByPackages.Sum(p => p.TotalSales);
            var averageOrderValue = totalSales > 0 ? totalRevenue / totalSales : 0;

            // Calculate period over period growth - handle empty data
            decimal periodOverPeriodGrowth = 0;
            if (timeSeries.Count > 1)
            {
                var currentPeriod = timeSeries[timeSeries.Count - 1];
                var previousPeriod = timeSeries[timeSeries.Count - 2];
                if (previousPeriod.TotalRevenue > 0)
                {
                    periodOverPeriodGrowth = ((currentPeriod.TotalRevenue - previousPeriod.TotalRevenue)
                        / previousPeriod.TotalRevenue) * 100;
                }
            }

            // Find top performing package - handle empty data
            var topPerformingPackage = revenueByPackages.FirstOrDefault();

            return new ComprehensiveRevenueReportDto
            {
                Summary = new RevenueReportSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalSales = totalSales,
                    AverageOrderValue = Math.Round(averageOrderValue, 2),
                    PeriodOverPeriodGrowth = Math.Round(periodOverPeriodGrowth, 2),
                    TopPerformingPackage = topPerformingPackage != null
                        ? new TopPerformingPackageDto
                        {
                            Name = topPerformingPackage.PackageName,
                            Revenue = topPerformingPackage.TotalRevenue,
                            Sales = topPerformingPackage.TotalSales
                        }
                        : new TopPerformingPackageDto
                        {
                            Name = "N/A",
                            Revenue = 0,
                            Sales = 0
                        }
                },
                RevenueByPackages = revenueByPackages,
                TimeSeries = timeSeries,
                Analytics = analytics
            };
        }

        public async Task<byte[]> ExportRevenueReportToExcelAsync(RevenueReportOptions options)
        {
            var report = await GetComprehensiveRevenueReportAsync(options);

            using var package = new ExcelPackage();

            // Summary Sheet
            var summarySheet = package.Workbook.Worksheets.Add("Revenue Summary");
            summarySheet.Cells["A1"].Value = "Revenue Report Summary";
            summarySheet.Cells["A1"].Style.Font.Bold = true;
            summarySheet.Cells["A1"].Style.Font.Size = 16;

            summarySheet.Cells["A2"].Value = "Total Revenue";
            summarySheet.Cells["B2"].Value = (double)report.Summary.TotalRevenue;
            summarySheet.Cells["B2"].Style.Numberformat.Format = "$#,##0.00";

            summarySheet.Cells["A3"].Value = "Total Sales";
            summarySheet.Cells["B3"].Value = report.Summary.TotalSales;

            summarySheet.Cells["A4"].Value = "Average Order Value";
            summarySheet.Cells["B4"].Value = (double)report.Summary.AverageOrderValue;
            summarySheet.Cells["B4"].Style.Numberformat.Format = "$#,##0.00";

            summarySheet.Cells["A5"].Value = "Period over Period Growth (%)";
            summarySheet.Cells["B5"].Value = (double)report.Summary.PeriodOverPeriodGrowth;
            summarySheet.Cells["B5"].Style.Numberformat.Format = "0.00";

            summarySheet.Cells["A7"].Value = "Top Performing Package";
            summarySheet.Cells["A7"].Style.Font.Bold = true;
            summarySheet.Cells["A8"].Value = "Package Name";
            summarySheet.Cells["B8"].Value = report.Summary.TopPerformingPackage?.Name ?? "N/A";
            summarySheet.Cells["A9"].Value = "Revenue";
            summarySheet.Cells["B9"].Value = (double)(report.Summary.TopPerformingPackage?.Revenue ?? 0);
            summarySheet.Cells["B9"].Style.Numberformat.Format = "$#,##0.00";
            summarySheet.Cells["A10"].Value = "Sales";
            summarySheet.Cells["B10"].Value = report.Summary.TopPerformingPackage?.Sales ?? 0;

            summarySheet.Cells["A12"].Value = "Analytics";
            summarySheet.Cells["A12"].Style.Font.Bold = true;
            summarySheet.Cells["A13"].Value = "Member Retention Rate (%)";
            summarySheet.Cells["B13"].Value = (double)report.Analytics.MemberRetentionRate;
            summarySheet.Cells["B13"].Style.Numberformat.Format = "0.00";
            summarySheet.Cells["A14"].Value = "Average Lifetime Value";
            summarySheet.Cells["B14"].Value = (double)report.Analytics.AverageLifetimeValue;
            summarySheet.Cells["B14"].Style.Numberformat.Format = "$#,##0.00";
            summarySheet.Cells["A15"].Value = "Churn Rate (%)";
            summarySheet.Cells["B15"].Value = (double)report.Analytics.ChurnRate;
            summarySheet.Cells["B15"].Style.Numberformat.Format = "0.00";

            summarySheet.Cells[summarySheet.Dimension?.Address ?? "A1:B15"].AutoFitColumns();

            // Revenue by Packages Sheet - handle empty data
            var packagesSheet = package.Workbook.Worksheets.Add("Revenue by Packages");
            packagesSheet.Cells["A1"].Value = "Package ID";
            packagesSheet.Cells["B1"].Value = "Package Name";
            packagesSheet.Cells["C1"].Value = "Category";
            packagesSheet.Cells["D1"].Value = "Total Revenue";
            packagesSheet.Cells["E1"].Value = "Total Sales";
            packagesSheet.Cells["F1"].Value = "Average Revenue";
            packagesSheet.Cells["A1:F1"].Style.Font.Bold = true;

            if (report.RevenueByPackages != null && report.RevenueByPackages.Any())
            {
                int row = 2;
                foreach (var pkg in report.RevenueByPackages)
                {
                    packagesSheet.Cells[row, 1].Value = pkg.PackageId ?? "";
                    packagesSheet.Cells[row, 2].Value = pkg.PackageName ?? "";
                    packagesSheet.Cells[row, 3].Value = pkg.Category ?? "";
                    packagesSheet.Cells[row, 4].Value = (double)pkg.TotalRevenue;
                    packagesSheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                    packagesSheet.Cells[row, 5].Value = pkg.TotalSales;
                    packagesSheet.Cells[row, 6].Value = (double)pkg.AverageRevenue;
                    packagesSheet.Cells[row, 6].Style.Numberformat.Format = "$#,##0.00";
                    row++;
                }
                packagesSheet.Cells[packagesSheet.Dimension.Address].AutoFitColumns();
            }
            else
            {
                packagesSheet.Cells["A2"].Value = "No data available";
            }

            // Time Series Sheet - handle empty data
            var timeSeriesSheet = package.Workbook.Worksheets.Add("Revenue Time Series");
            timeSeriesSheet.Cells["A1"].Value = "Period";
            timeSeriesSheet.Cells["B1"].Value = "Total Revenue";
            timeSeriesSheet.Cells["C1"].Value = "Total Sales";
            timeSeriesSheet.Cells["A1:C1"].Style.Font.Bold = true;

            if (report.TimeSeries != null && report.TimeSeries.Any())
            {
                int row = 2;
                foreach (var item in report.TimeSeries)
                {
                    timeSeriesSheet.Cells[row, 1].Value = item.Period ?? "";
                    timeSeriesSheet.Cells[row, 2].Value = (double)item.TotalRevenue;
                    timeSeriesSheet.Cells[row, 2].Style.Numberformat.Format = "$#,##0.00";
                    timeSeriesSheet.Cells[row, 3].Value = item.TotalSales;
                    row++;
                }
                timeSeriesSheet.Cells[timeSeriesSheet.Dimension.Address].AutoFitColumns();
            }
            else
            {
                timeSeriesSheet.Cells["A2"].Value = "No data available";
            }

            // Package Popularity Sheet - handle empty data
            var popularitySheet = package.Workbook.Worksheets.Add("Package Popularity");
            popularitySheet.Cells["A1"].Value = "Package Name";
            popularitySheet.Cells["B1"].Value = "Member Count";
            popularitySheet.Cells["C1"].Value = "Percentage";
            popularitySheet.Cells["A1:C1"].Style.Font.Bold = true;

            if (report.Analytics?.PackagePopularity != null && report.Analytics.PackagePopularity.Any())
            {
                int row = 2;
                foreach (var pkg in report.Analytics.PackagePopularity)
                {
                    popularitySheet.Cells[row, 1].Value = pkg.PackageName ?? "";
                    popularitySheet.Cells[row, 2].Value = pkg.MemberCount;
                    popularitySheet.Cells[row, 3].Value = (double)pkg.Percentage;
                    popularitySheet.Cells[row, 3].Style.Numberformat.Format = "0.00\"%\"";
                    row++;
                }
                popularitySheet.Cells[popularitySheet.Dimension.Address].AutoFitColumns();
            }
            else
            {
                popularitySheet.Cells["A2"].Value = "No data available";
            }

            // Payment Method Sheet - handle empty data
            var paymentMethodSheet = package.Workbook.Worksheets.Add("Revenue by Payment Method");
            paymentMethodSheet.Cells["A1"].Value = "Payment Method";
            paymentMethodSheet.Cells["B1"].Value = "Revenue";
            paymentMethodSheet.Cells["C1"].Value = "Percentage";
            paymentMethodSheet.Cells["A1:C1"].Style.Font.Bold = true;

            if (report.Analytics?.RevenueByPaymentMethod != null && report.Analytics.RevenueByPaymentMethod.Any())
            {
                int row = 2;
                foreach (var method in report.Analytics.RevenueByPaymentMethod)
                {
                    paymentMethodSheet.Cells[row, 1].Value = method.Method ?? "";
                    paymentMethodSheet.Cells[row, 2].Value = (double)method.Revenue;
                    paymentMethodSheet.Cells[row, 2].Style.Numberformat.Format = "$#,##0.00";
                    paymentMethodSheet.Cells[row, 3].Value = (double)method.Percentage;
                    paymentMethodSheet.Cells[row, 3].Style.Numberformat.Format = "0.00\"%\"";
                    row++;
                }
                paymentMethodSheet.Cells[paymentMethodSheet.Dimension.Address].AutoFitColumns();
            }
            else
            {
                paymentMethodSheet.Cells["A2"].Value = "No data available";
            }

            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> ExportRevenueReportToPdfAsync(RevenueReportOptions options)
        {
            var report = await GetComprehensiveRevenueReportAsync(options);

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
                    <h1>Revenue Analytics Report</h1>
                    
                    <h2>Revenue Summary</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>Total Revenue:</strong> ${report.Summary.TotalRevenue:N2}</div>
                        <div class='summary-item'><strong>Total Sales:</strong> {report.Summary.TotalSales}</div>
                        <div class='summary-item'><strong>Average Order Value:</strong> ${report.Summary.AverageOrderValue:N2}</div>
                        <div class='summary-item'><strong>Period over Period Growth:</strong> {report.Summary.PeriodOverPeriodGrowth:N2}%</div>
                    </div>

                    <h2>Top Performing Package</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>Package:</strong> {report.Summary.TopPerformingPackage.Name}</div>
                        <div class='summary-item'><strong>Revenue:</strong> ${report.Summary.TopPerformingPackage.Revenue:N2}</div>
                        <div class='summary-item'><strong>Sales:</strong> {report.Summary.TopPerformingPackage.Sales}</div>
                    </div>

                    <h2>Key Analytics</h2>
                    <div class='summary'>
                        <div class='summary-item'><strong>Member Retention Rate:</strong> {report.Analytics.MemberRetentionRate:N2}%</div>
                        <div class='summary-item'><strong>Average Lifetime Value:</strong> ${report.Analytics.AverageLifetimeValue:N2}</div>
                        <div class='summary-item'><strong>Churn Rate:</strong> {report.Analytics.ChurnRate:N2}%</div>
                    </div>

                    <h2>Revenue by Package</h2>
                    <table>
                        <tr>
                            <th>Package Name</th>
                            <th>Category</th>
                            <th>Revenue</th>
                            <th>Sales</th>
                        </tr>";

            foreach (var pkg in report.RevenueByPackages.Take(10))
            {
                html += $@"
                        <tr>
                            <td>{pkg.PackageName}</td>
                            <td>{pkg.Category}</td>
                            <td>${pkg.TotalRevenue:N2}</td>
                            <td>{pkg.TotalSales}</td>
                        </tr>";
            }

            html += @"
                    </table>

                    <h2>Revenue by Payment Method</h2>
                    <table>
                        <tr>
                            <th>Payment Method</th>
                            <th>Revenue</th>
                            <th>Percentage</th>
                        </tr>";

            foreach (var method in report.Analytics.RevenueByPaymentMethod)
            {
                html += $@"
                        <tr>
                            <td>{method.Method}</td>
                            <td>${method.Revenue:N2}</td>
                            <td>{method.Percentage:N1}%</td>
                        </tr>";
            }

            html += @"
                    </table>
                </body>
                </html>";

            var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            using var stream = new MemoryStream();
            pdf.Save(stream, false);
            return stream.ToArray();
        }

        public async Task<RevenueDashboardDto> GetRevenueDashboardAsync(int period)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-period);

            var groupBy = period <= 7 ? "day" : period <= 31 ? "week" : "month";

            var options = new RevenueReportOptions
            {
                StartDate = startDate,
                EndDate = endDate,
                GroupBy = groupBy
            };

            var revenueByPackages = await GetRevenueByPackagesAsync(options);
            var timeSeries = await GetRevenueTimeSeriesAsync(options);
            var analytics = await GetAdvancedAnalyticsAsync(startDate, endDate);

            var totalRevenue = revenueByPackages.Sum(p => p.TotalRevenue);
            var totalSales = revenueByPackages.Sum(p => p.TotalSales);
            var averageOrderValue = totalSales > 0 ? totalRevenue / totalSales : 0;

            // Calculate growth rate
            decimal growthRate = 0;
            if (timeSeries.Count >= 2)
            {
                var currentPeriod = timeSeries[timeSeries.Count - 1];
                var previousPeriod = timeSeries[timeSeries.Count - 2];
                if (previousPeriod.TotalRevenue > 0)
                {
                    growthRate = ((currentPeriod.TotalRevenue - previousPeriod.TotalRevenue)
                        / previousPeriod.TotalRevenue) * 100;
                }
            }

            return new RevenueDashboardDto
            {
                Summary = new DashboardSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalSales = totalSales,
                    AverageOrderValue = Math.Round(averageOrderValue, 2),
                    GrowthRate = Math.Round(growthRate, 2),
                    Period = $"Last {period} days"
                },
                TopPackages = revenueByPackages.Take(5).ToList(),
                RecentTrends = timeSeries.TakeLast(7).ToList(),
                Analytics = new DashboardAnalyticsDto
                {
                    MemberRetentionRate = analytics.MemberRetentionRate,
                    ChurnRate = analytics.ChurnRate,
                    TopPaymentMethods = analytics.RevenueByPaymentMethod.Take(3).ToList()
                }
            };
        }
    }
}