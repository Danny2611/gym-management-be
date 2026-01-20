using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Progress
{
    // Response DTOs
    public class ProgressResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member_id")]
        public string MemberId { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        [JsonPropertyName("height")]
        public double Height { get; set; }

        [JsonPropertyName("muscle_mass")]
        public double MuscleMass { get; set; }

        [JsonPropertyName("body_fat")]
        public double BodyFat { get; set; }

        [JsonPropertyName("bmi")]
        public double Bmi { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    // Request DTOs
    public class UpdateBodyMetricsDto
    {
        [JsonPropertyName("weight")]
        public double? Weight { get; set; }

        [JsonPropertyName("height")]
        public double? Height { get; set; }

        [JsonPropertyName("muscle_mass")]
        public double? MuscleMass { get; set; }

        [JsonPropertyName("body_fat")]
        public double? BodyFat { get; set; }
    }

    // Comparison DTOs
    public class BodyMetricsComparisonDto
    {
        [JsonPropertyName("current")]
        public ProgressResponseDto Current { get; set; }

        [JsonPropertyName("initial")]
        public ProgressResponseDto Initial { get; set; }

        [JsonPropertyName("previous")]
        public ProgressResponseDto Previous { get; set; }
    }

    // Monthly Stats DTOs
    public class MonthlyBodyMetricsDto
    {
        [JsonPropertyName("month")]
        public string Month { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        [JsonPropertyName("body_fat")]
        public double BodyFat { get; set; }

        [JsonPropertyName("muscle_mass")]
        public double MuscleMass { get; set; }

        [JsonPropertyName("bmi")]
        public double Bmi { get; set; }
    }

    // Fitness Radar DTOs
    public class FitnessRadarDto
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("current")]
        public double Current { get; set; }

        [JsonPropertyName("initial")]
        public double Initial { get; set; }

        [JsonPropertyName("full")]
        public double Full { get; set; }
    }

    // Metrics Change DTOs
    public class MetricsChangeDto
    {
        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("body_fat")]
        public string BodyFat { get; set; }

        [JsonPropertyName("muscle_mass")]
        public string MuscleMass { get; set; }

        [JsonPropertyName("bmi")]
        public string Bmi { get; set; }
    }

    public class BodyMetricsChangeResponseDto
    {
        [JsonPropertyName("changes")]
        public MetricsChangeDto Changes { get; set; }

        [JsonPropertyName("current")]
        public ProgressResponseDto Current { get; set; }

        [JsonPropertyName("initial")]
        public ProgressResponseDto Initial { get; set; }
    }
}