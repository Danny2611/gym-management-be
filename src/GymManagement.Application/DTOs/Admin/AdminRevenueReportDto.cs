using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Query Options
    public class RevenueReportOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string GroupBy { get; set; } = "month"; // day, week, month, year
        public string? PackageId { get; set; }
        public string? Category { get; set; } // basic, fitness, premium, platinum, vip
    }

    // Revenue by Package Response
    public class RevenueByPackageDto
    {
        [JsonPropertyName("package_id")]
        public string PackageId { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("total_sales")]
        public int TotalSales { get; set; }

        [JsonPropertyName("average_revenue")]
        public decimal AverageRevenue { get; set; }
    }

    // Revenue Time Series Response
    public class RevenueTimeSeriesDto
    {
        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("total_sales")]
        public int TotalSales { get; set; }

        [JsonPropertyName("packages")]
        public List<TimeSeriesPackageDto> Packages { get; set; }
    }

    public class TimeSeriesPackageDto
    {
        [JsonPropertyName("package_id")]
        public string PackageId { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("sales")]
        public int Sales { get; set; }
    }

    // Advanced Analytics Response
    public class AdvancedAnalyticsDto
    {
        [JsonPropertyName("member_retention_rate")]
        public decimal MemberRetentionRate { get; set; }

        [JsonPropertyName("average_lifetime_value")]
        public decimal AverageLifetimeValue { get; set; }

        [JsonPropertyName("churn_rate")]
        public decimal ChurnRate { get; set; }

        [JsonPropertyName("package_popularity")]
        public List<PackagePopularityDto> PackagePopularity { get; set; }

        [JsonPropertyName("revenue_by_payment_method")]
        public List<RevenueByPaymentMethodDto> RevenueByPaymentMethod { get; set; }
    }

    public class PackagePopularityDto
    {
        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }
    }

    public class RevenueByPaymentMethodDto
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
    }

    // Comprehensive Revenue Report
    public class ComprehensiveRevenueReportDto
    {
        [JsonPropertyName("summary")]
        public RevenueReportSummaryDto Summary { get; set; }

        [JsonPropertyName("revenue_by_packages")]
        public List<RevenueByPackageDto> RevenueByPackages { get; set; }

        [JsonPropertyName("time_series")]
        public List<RevenueTimeSeriesDto> TimeSeries { get; set; }

        [JsonPropertyName("analytics")]
        public AdvancedAnalyticsDto Analytics { get; set; }
    }

    public class RevenueReportSummaryDto
    {
        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("total_sales")]
        public int TotalSales { get; set; }

        [JsonPropertyName("average_order_value")]
        public decimal AverageOrderValue { get; set; }

        [JsonPropertyName("period_over_period_growth")]
        public decimal PeriodOverPeriodGrowth { get; set; }

        [JsonPropertyName("top_performing_package")]
        public TopPerformingPackageDto TopPerformingPackage { get; set; }
    }

    public class TopPerformingPackageDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("sales")]
        public int Sales { get; set; }
    }

    // Revenue Dashboard Response
    public class RevenueDashboardDto
    {
        [JsonPropertyName("summary")]
        public DashboardSummaryDto Summary { get; set; }

        [JsonPropertyName("top_packages")]
        public List<RevenueByPackageDto> TopPackages { get; set; }

        [JsonPropertyName("recent_trends")]
        public List<RevenueTimeSeriesDto> RecentTrends { get; set; }

        [JsonPropertyName("analytics")]
        public DashboardAnalyticsDto Analytics { get; set; }
    }

    public class DashboardSummaryDto
    {
        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("total_sales")]
        public int TotalSales { get; set; }

        [JsonPropertyName("average_order_value")]
        public decimal AverageOrderValue { get; set; }

        [JsonPropertyName("growth_rate")]
        public decimal GrowthRate { get; set; }

        [JsonPropertyName("period")]
        public string Period { get; set; }
    }

    public class DashboardAnalyticsDto
    {
        [JsonPropertyName("member_retention_rate")]
        public decimal MemberRetentionRate { get; set; }

        [JsonPropertyName("churn_rate")]
        public decimal ChurnRate { get; set; }

        [JsonPropertyName("top_payment_methods")]
        public List<RevenueByPaymentMethodDto> TopPaymentMethods { get; set; }
    }
}