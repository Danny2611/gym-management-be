using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Workout
{
    // Response DTOs
    public class WorkoutScheduleResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("time_start")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("muscle_groups")]
        public List<string> MuscleGroups { get; set; }

        [JsonPropertyName("exercises")]
        public List<ExerciseDto> Exercises { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class ExerciseDto
    {
        // [JsonPropertyName("id")]
        // public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sets")]
        public int Sets { get; set; }

        [JsonPropertyName("reps")]
        public int Reps { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }
    }

    // Request DTOs
    public class CreateWorkoutScheduleDto
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("time_start")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("muscle_groups")]
        public List<string> MuscleGroups { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        [JsonPropertyName("exercises")]
        public List<ExerciseDto> Exercises { get; set; }


    }

    public class UpdateWorkoutStatusDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    // Statistics DTOs
    public class WeeklyWorkoutDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sessions")]
        public int Sessions { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("target")]
        public int Target { get; set; }
    }

    public class MonthlyStatsDto
    {
        [JsonPropertyName("total_sessions")]
        public int TotalSessions { get; set; }

        [JsonPropertyName("total_duration")]
        public int TotalDuration { get; set; }

        [JsonPropertyName("completion_rate")]
        public int CompletionRate { get; set; }

        [JsonPropertyName("avg_session_length")]
        public int AvgSessionLength { get; set; }
    }

    public class MonthComparisonDto
    {
        [JsonPropertyName("current")]
        public MonthlyStatsDto Current { get; set; }

        [JsonPropertyName("previous")]
        public MonthlyStatsDto Previous { get; set; }
    }

    public class RecentWorkoutLogDto
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("muscle_groups")]
        public List<string> MuscleGroups { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class WorkoutScheduleNextWeekDto
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("time_start")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("time_end")]
        public DateTime TimeEnd { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}