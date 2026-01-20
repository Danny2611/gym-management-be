using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Dashboard Stats Response
    public class AdminDashboardStatsDto
    {
        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("total_members")]
        public int TotalMembers { get; set; }

        [JsonPropertyName("active_members")]
        public int ActiveMembers { get; set; }

        [JsonPropertyName("expired_memberships")]
        public int ExpiredMemberships { get; set; }

        [JsonPropertyName("revenue_growth")]
        public decimal RevenueGrowth { get; set; }

        [JsonPropertyName("member_growth")]
        public decimal MemberGrowth { get; set; }

        [JsonPropertyName("top_packages")]
        public List<AdminTopPackageDto> TopPackages { get; set; }

        [JsonPropertyName("recent_payments")]
        public List<AdminRecentPaymentDto> RecentPayments { get; set; }
    }

    public class AdminTopPackageDto
    {
        [JsonPropertyName("package_id")]
        public string PackageId { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }
    }

    public class AdminRecentPaymentDto
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("member_name")]
        public string MemberName { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    // Advanced Analytics Response
    public class AdminAdvancedAnalyticsDto
    {
        [JsonPropertyName("member_retention_rate")]
        public decimal MemberRetentionRate { get; set; }

        [JsonPropertyName("average_lifetime_value")]
        public decimal AverageLifetimeValue { get; set; }

        [JsonPropertyName("churn_rate")]
        public decimal ChurnRate { get; set; }

        [JsonPropertyName("package_popularity")]
        public List<AdminPackagePopularityDto> PackagePopularity { get; set; }

        [JsonPropertyName("revenue_by_payment_method")]
        public List<AdminRevenueByPaymentMethodDto> RevenueByPaymentMethod { get; set; }
    }

    public class AdminPackagePopularityDto
    {
        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }
    }

    public class AdminRevenueByPaymentMethodDto
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
    }

    // Date Range Helper
    public class AdminReportDateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}