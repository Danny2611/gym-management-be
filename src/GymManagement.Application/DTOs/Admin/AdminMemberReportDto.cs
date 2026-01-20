using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Query Options
    public class AdminMemberStatsOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string GroupBy { get; set; } = "month"; // day, week, month, year
        public string? Status { get; set; } // active, inactive, pending, banned
        public bool IncludeRetention { get; set; } = false;
        public bool IncludeChurn { get; set; } = false;
        public bool CohortAnalysis { get; set; } = false;
    }

    // Member Stats Response
    public class AdminMemberStatsDto
    {
        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("total_members")]
        public int TotalMembers { get; set; }

        [JsonPropertyName("new_members")]
        public int NewMembers { get; set; }

        [JsonPropertyName("expired_members")]
        public int ExpiredMembers { get; set; }

        [JsonPropertyName("active_members")]
        public int ActiveMembers { get; set; }

        [JsonPropertyName("inactive_members")]
        public int InactiveMembers { get; set; }

        [JsonPropertyName("pending_members")]
        public int PendingMembers { get; set; }

        [JsonPropertyName("banned_members")]
        public int BannedMembers { get; set; }

        [JsonPropertyName("retention_rate")]
        public decimal? RetentionRate { get; set; }

        [JsonPropertyName("churn_rate")]
        public decimal? ChurnRate { get; set; }

        [JsonPropertyName("growth_rate")]
        public decimal? GrowthRate { get; set; }

        [JsonPropertyName("net_growth")]
        public int? NetGrowth { get; set; }
    }

    // Retention Analysis
    public class AdminRetentionAnalysisDto
    {
        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("cohort_size")]
        public int CohortSize { get; set; }

        [JsonPropertyName("retained_members")]
        public int RetainedMembers { get; set; }

        [JsonPropertyName("retention_rate")]
        public decimal RetentionRate { get; set; }

        [JsonPropertyName("churned_members")]
        public int ChurnedMembers { get; set; }

        [JsonPropertyName("churn_rate")]
        public decimal ChurnRate { get; set; }
    }

    // Member Growth Analysis
    public class AdminMemberGrowthAnalysisDto
    {
        [JsonPropertyName("total_members")]
        public int TotalMembers { get; set; }

        [JsonPropertyName("total_growth")]
        public int TotalGrowth { get; set; }

        [JsonPropertyName("growth_rate")]
        public decimal GrowthRate { get; set; }

        [JsonPropertyName("period_over_period_growth")]
        public decimal PeriodOverPeriodGrowth { get; set; }

        [JsonPropertyName("retention_funnel")]
        public AdminRetentionFunnelDto RetentionFunnel { get; set; }
    }

    public class AdminRetentionFunnelDto
    {
        [JsonPropertyName("new_members")]
        public int NewMembers { get; set; }

        [JsonPropertyName("active_after_30_days")]
        public int ActiveAfter30Days { get; set; }

        [JsonPropertyName("active_after_90_days")]
        public int ActiveAfter90Days { get; set; }

        [JsonPropertyName("active_after_180_days")]
        public int ActiveAfter180Days { get; set; }

        [JsonPropertyName("active_after_365_days")]
        public int ActiveAfter365Days { get; set; }
    }

    // Status Distribution
    public class AdminStatusDistributionDto
    {
        [JsonPropertyName("active")]
        public AdminStatusItemDto Active { get; set; }

        [JsonPropertyName("inactive")]
        public AdminStatusItemDto Inactive { get; set; }

        [JsonPropertyName("pending")]
        public AdminStatusItemDto Pending { get; set; }

        [JsonPropertyName("banned")]
        public AdminStatusItemDto Banned { get; set; }
    }

    public class AdminStatusItemDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }
    }

    // Churn Analysis
    public class AdminChurnAnalysisDto
    {
        [JsonPropertyName("average_churn_rate")]
        public decimal AverageChurnRate { get; set; }

        [JsonPropertyName("high_risk_periods")]
        public List<string> HighRiskPeriods { get; set; }

        [JsonPropertyName("churn_reasons")]
        public List<AdminChurnReasonDto> ChurnReasons { get; set; }
    }

    public class AdminChurnReasonDto
    {
        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    // Top Growth Periods
    public class AdminTopGrowthPeriodDto
    {
        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("growth")]
        public int Growth { get; set; }
    }

    // Comprehensive Member Report
    public class AdminComprehensiveMemberReportDto
    {
        [JsonPropertyName("summary")]
        public AdminMemberGrowthAnalysisDto Summary { get; set; }

        [JsonPropertyName("time_series")]
        public List<AdminMemberStatsDto> TimeSeries { get; set; }

        [JsonPropertyName("status_distribution")]
        public AdminStatusDistributionDto StatusDistribution { get; set; }

        [JsonPropertyName("retention_analysis")]
        public List<AdminRetentionAnalysisDto> RetentionAnalysis { get; set; }

        [JsonPropertyName("top_growth_periods")]
        public List<AdminTopGrowthPeriodDto> TopGrowthPeriods { get; set; }

        [JsonPropertyName("churn_analysis")]
        public AdminChurnAnalysisDto ChurnAnalysis { get; set; }
    }
}