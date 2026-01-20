using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Query Options
    public class AppointmentQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; } // confirmed, pending, cancelled, completed, missed
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? MemberId { get; set; }
        public string? TrainerId { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } // asc, desc
    }

    // Response DTOs
    public class AppointmentResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member")]
        public AppointmentMemberDto Member { get; set; }

        [JsonPropertyName("trainer")]
        public AppointmentTrainerDto Trainer { get; set; }

        [JsonPropertyName("membership_id")]
        public string MembershipId { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("time")]
        public AppointmentTimeDto Time { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class AppointmentMemberDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class AppointmentTrainerDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class AppointmentTimeDto
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }

    // Request DTOs
    public class UpdateAppointmentStatusDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } // confirmed, pending, cancelled
    }

    // List Response DTO
    public class AppointmentListResponseDto
    {
        [JsonPropertyName("appointments")]
        public List<AppointmentResponseDto> Appointments { get; set; }

        [JsonPropertyName("total_appointments")]
        public int TotalAppointments { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Stats Response DTO
    public class AppointmentStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("confirmed")]
        public int Confirmed { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("cancelled")]
        public int Cancelled { get; set; }

        [JsonPropertyName("completed")]
        public int Completed { get; set; }

        [JsonPropertyName("missed")]
        public int Missed { get; set; }

        [JsonPropertyName("upcoming_today")]
        public int UpcomingToday { get; set; }

        [JsonPropertyName("upcoming_week")]
        public int UpcomingWeek { get; set; }
    }
}