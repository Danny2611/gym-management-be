using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin.Trainer
{
    // Response DTOs
    public class TrainerResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("specialization")]
        public string Specialization { get; set; }

        [JsonPropertyName("experience")]
        public int Experience { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("schedule")]
        public List<TrainerScheduleDto> Schedule { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class TrainerScheduleDto
    {
        [JsonPropertyName("dayOfWeek")]
        public int DayOfWeek { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }

        [JsonPropertyName("workingHours")]
        public List<WorkingHourDto> WorkingHours { get; set; }
    }

    public class WorkingHourDto
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

    // Request DTOs
    public class CreateTrainerDto
    {
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("specialization")]
        public string? Specialization { get; set; }

        [JsonPropertyName("experience")]
        public int? Experience { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("schedule")]
        public List<TrainerScheduleDto>? Schedule { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class UpdateTrainerDto
    {
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("specialization")]
        public string? Specialization { get; set; }

        [JsonPropertyName("experience")]
        public int? Experience { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class UpdateScheduleDto
    {
        [JsonPropertyName("schedule")]
        public List<TrainerScheduleDto> Schedule { get; set; }
    }

    // List Response
    public class TrainerListResponseDto
    {
        [JsonPropertyName("trainers")]
        public List<TrainerResponseDto> Trainers { get; set; }

        [JsonPropertyName("total_trainers")]
        public int TotalTrainers { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Availability Response
    public class TrainerAvailabilityDto
    {
        [JsonPropertyName("trainer")]
        public TrainerResponseDto Trainer { get; set; }

        [JsonPropertyName("available_dates")]
        public List<AvailableDateDto> AvailableDates { get; set; }
    }

    public class AvailableDateDto
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("dayOfWeek")]
        public int DayOfWeek { get; set; }

        [JsonPropertyName("workingHours")]
        public List<WorkingHourDto> WorkingHours { get; set; }
    }

    // Statistics
    public class TrainerStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }

        [JsonPropertyName("by_specialization")]
        public Dictionary<string, int> BySpecialization { get; set; }

        [JsonPropertyName("experience_ranges")]
        public ExperienceRangesDto ExperienceRanges { get; set; }
    }

    public class ExperienceRangesDto
    {
        [JsonPropertyName("novice")]
        public int Novice { get; set; }

        [JsonPropertyName("intermediate")]
        public int Intermediate { get; set; }

        [JsonPropertyName("experienced")]
        public int Experienced { get; set; }

        [JsonPropertyName("expert")]
        public int Expert { get; set; }
    }

    // Query Options
    public class TrainerQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Search { get; set; }
        public string Status { get; set; }
        public string Specialization { get; set; }
        public int? Experience { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}