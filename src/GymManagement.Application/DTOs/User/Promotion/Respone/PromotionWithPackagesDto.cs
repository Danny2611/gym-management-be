using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Promotion
{
    // DTO cho Package info (chỉ lấy một số field)
    public class PackageInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> Benefits { get; set; }
    }



    // DTO cho Promotion response với populated packages
    public class PromotionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime Start_date { get; set; }
        [JsonPropertyName("end_date")]
        public DateTime End_date { get; set; }
        [JsonPropertyName("applicable_packages")]
        public List<PackageInfoDto> ApplicablePackages { get; set; } = new();
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime Created_at { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime Updated_at { get; set; }
    }

    // DTO cho Promotion response không populate (chỉ IDs)
    public class PromotionSimpleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> ApplicablePackages { get; set; } = new();
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }



}