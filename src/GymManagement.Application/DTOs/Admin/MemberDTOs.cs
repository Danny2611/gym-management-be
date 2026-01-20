using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Response DTOs
    public class MemberResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("role")]
        public MemberRoleDto Role { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MemberRoleDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // Request DTOs
    public class CreateMemberDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isVerified")]
        public bool? IsVerified { get; set; }
    }

    public class UpdateMemberDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("role_id")]
        public string RoleId { get; set; }

        [JsonPropertyName("isVerified")]
        public bool? IsVerified { get; set; }
    }

    public class UpdateMemberStatusDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    // Pagination Response
    public class MemberListResponseDto
    {
        [JsonPropertyName("members")]
        public List<MemberResponseDto> Members { get; set; }

        [JsonPropertyName("total_members")]
        public int TotalMembers { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Statistics Response
    public class MemberStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("banned")]
        public int Banned { get; set; }

        [JsonPropertyName("verified")]
        public int Verified { get; set; }

        [JsonPropertyName("unverified")]
        public int Unverified { get; set; }
    }

    // Query Options
    public class MemberQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Search { get; set; }
        public string Status { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}