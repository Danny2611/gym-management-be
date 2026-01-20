using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Admin
{
    // Response DTOs
    public class PackageResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("max_members")]
        public int? MaxMembers { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("benefits")]
        public List<string> Benefits { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("popular")]
        public bool Popular { get; set; }

        [JsonPropertyName("training_sessions")]
        public int TrainingSessions { get; set; }

        [JsonPropertyName("session_duration")]
        public int SessionDuration { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("package_detail")]
        public PackageDetailDto PackageDetail { get; set; }
    }

    public class PackageDetailDto
    {
        [JsonPropertyName("schedule")]
        public List<string> Schedule { get; set; }

        [JsonPropertyName("training_areas")]
        public List<string> TrainingAreas { get; set; }

        [JsonPropertyName("additional_services")]
        public List<string> AdditionalServices { get; set; }
    }

    // Request DTOs
    public class CreatePackageDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("max_members")]
        public int? MaxMembers { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("benefits")]
        public List<string> Benefits { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("popular")]
        public bool? Popular { get; set; }

        [JsonPropertyName("training_sessions")]
        public int? TrainingSessions { get; set; }

        [JsonPropertyName("session_duration")]
        public int? SessionDuration { get; set; }

        [JsonPropertyName("packageDetail")]
        public PackageDetailInputDto? PackageDetail { get; set; }
    }

    public class UpdatePackageDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("max_members")]
        public int? MaxMembers { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("benefits")]
        public List<string> Benefits { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("popular")]
        public bool? Popular { get; set; }

        [JsonPropertyName("training_sessions")]
        public int? TrainingSessions { get; set; }

        [JsonPropertyName("session_duration")]
        public int? SessionDuration { get; set; }

        [JsonPropertyName("packageDetail")]
        public PackageDetailInputDto? PackageDetail { get; set; }
    }

    public class PackageDetailInputDto
    {
        [JsonPropertyName("schedule")]
        public List<string> Schedule { get; set; }

        [JsonPropertyName("training_areas")]
        public List<string> TrainingAreas { get; set; }

        [JsonPropertyName("additional_services")]
        public List<string> AdditionalServices { get; set; }
    }

    // List Response DTO
    public class PackageListResponseDto
    {
        [JsonPropertyName("packages")]
        public List<PackageResponseDto> Packages { get; set; }

        [JsonPropertyName("total_packages")]
        public int TotalPackages { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Statistics DTO
    public class PackageStatsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("active")]
        public int Active { get; set; }

        [JsonPropertyName("inactive")]
        public int Inactive { get; set; }

        [JsonPropertyName("by_category")]
        public Dictionary<string, int> ByCategory { get; set; }

        [JsonPropertyName("popular")]
        public int Popular { get; set; }

        [JsonPropertyName("with_training_sessions")]
        public int WithTrainingSessions { get; set; }
    }

    // Query Options
    public class PackageQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Search { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public bool? Popular { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
    }
}