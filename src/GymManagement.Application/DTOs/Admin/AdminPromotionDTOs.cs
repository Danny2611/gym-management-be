using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Query Options
    public class PromotionQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; } // active, inactive
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } // asc, desc
    }

    // Response DTOs
    public class PromotionResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("applicable_packages")]
        public List<PromotionPackageDto> ApplicablePackages { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class PromotionPackageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    // Request DTOs
    public class CreatePromotionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "active";

        [JsonPropertyName("applicable_packages")]
        public List<string> ApplicablePackages { get; set; }
    }

    public class UpdatePromotionDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("discount")]
        public decimal? Discount { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("applicable_packages")]
        public List<string>? ApplicablePackages { get; set; }
    }

    // List Response DTO
    public class PromotionListResponseDto
    {
        [JsonPropertyName("promotions")]
        public List<PromotionResponseDto> Promotions { get; set; }

        [JsonPropertyName("total_promotions")]
        public int TotalPromotions { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Stats Response DTO
    public class PromotionStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }

        [JsonPropertyName("expired_this_month")]
        public int ExpiredThisMonth { get; set; }

        [JsonPropertyName("upcoming_this_month")]
        public int UpcomingThisMonth { get; set; }
    }

    // Effectiveness Response DTO
    public class PromotionEffectivenessDto
    {
        [JsonPropertyName("promotion_id")]
        public string PromotionId { get; set; }

        [JsonPropertyName("promotion_name")]
        public string PromotionName { get; set; }

        [JsonPropertyName("promotion_period")]
        public PromotionPeriodDto PromotionPeriod { get; set; }

        [JsonPropertyName("package_stats")]
        public List<PromotionPackageStatsDto> PackageStats { get; set; }

        [JsonPropertyName("total_memberships")]
        public int TotalMemberships { get; set; }

        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("average_conversion_rate")]
        public decimal AverageConversionRate { get; set; }
    }

    public class PromotionPeriodDto
    {
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
    }

    public class PromotionPackageStatsDto
    {
        [JsonPropertyName("package_id")]
        public string PackageId { get; set; }

        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }

        [JsonPropertyName("total_memberships")]
        public int TotalMemberships { get; set; }

        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("conversion_rate")]
        public decimal ConversionRate { get; set; }
    }
}