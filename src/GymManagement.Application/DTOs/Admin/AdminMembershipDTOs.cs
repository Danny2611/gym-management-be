using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{


    // Response DTOs
    public class MembershipResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member")]
        public MembershipMemberDto Member { get; set; }

        [JsonPropertyName("package")]
        public MembershipPackageDto Package { get; set; }

        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("auto_renew")]
        public bool AutoRenew { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("available_sessions")]
        public int AvailableSessions { get; set; }

        [JsonPropertyName("used_sessions")]
        public int UsedSessions { get; set; }

        [JsonPropertyName("last_sessions_reset")]
        public DateTime? LastSessionsReset { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MembershipMemberDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class MembershipPackageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("training_sessions")]
        public int TrainingSessions { get; set; }
    }

    // Request DTOs
    public class DeleteMembershipDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    // List Response DTO
    public class MembershipListResponseDto
    {
        [JsonPropertyName("memberships")]
        public List<MembershipResponseDto> Memberships { get; set; }

        [JsonPropertyName("total_memberships")]
        public int TotalMemberships { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Statistics DTO
    public class MembershipStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("expired")]
        public int Expired { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("paused")]
        public int Paused { get; set; }

        [JsonPropertyName("auto_renew")]
        public int AutoRenew { get; set; }

        [JsonPropertyName("available_sessions")]
        public int AvailableSessions { get; set; }

        [JsonPropertyName("used_sessions")]
        public int UsedSessions { get; set; }
    }

    // Query Options
    public class MembershipQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Search { get; set; }
        public string Status { get; set; }
        public string MemberId { get; set; }
        public string PackageId { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}