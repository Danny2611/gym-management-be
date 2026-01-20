using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User
{
    public class MembershipResponse
    {
        public string Id { get; set; }
        public string Member_id { get; set; }
        public PackageInfoDto Package_id { get; set; }
        public PaymentInfoDto Payment_id { get; set; }
        public DateTime? Start_date { get; set; }
        public DateTime? End_date { get; set; }
        public bool AutoRenew { get; set; }
        public string Status { get; set; }
        public int Available_sessions { get; set; }
        public int Used_sessions { get; set; }
        public DateTime? Last_sessions_reset { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }

        // Calculated fields
        public int RemainingDays { get; set; }
        public double RemainingPercent { get; set; }
    }

    public class PackageInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int Max_members { get; set; }
        public string Description { get; set; }
        public List<string> Benefits { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public bool? Popular { get; set; }
        public int? Training_sessions { get; set; }
        public int? Session_duration { get; set; }
        public PackageDetailsDto Details { get; set; }
    }

    public class PackageDetailsDto
    {
        public string Id { get; set; }
        public string Package_id { get; set; }
        public List<string> Schedule { get; set; }
        public List<string> Training_areas { get; set; }
        public List<string> Additional_services { get; set; }
        public string Status { get; set; }
    }

    public class PaymentInfoDto
    {
        public string Id { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime Payment_date { get; set; }
        public string Status { get; set; }
        public PaymentInfoDetailsDto PaymentInfo { get; set; }
    }

    public class PaymentInfoDetailsDto
    {
        public string ResponseTime { get; set; }
        public string Message { get; set; }
    }

    // Response đăng ký gói tập
    public class RegisterPackageResponse
    {
        public string PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Category { get; set; }
    }

    // Response chi tiết membership
    public class MembershipDetailsResponse
    {
        [JsonPropertyName("membership_id")]
        public string MembershipId { get; set; }

        [JsonPropertyName("member_name")]
        public string MemberName { get; set; }
        [JsonPropertyName("member_avatar")]
        public string MemberAvatar { get; set; }
        [JsonPropertyName("package_id")]
        public string PackageId { get; set; }
        [JsonPropertyName("package_name")]
        public string PackageName { get; set; }
        [JsonPropertyName("package_category")]
        public string PackageCategory { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("days_remaining")]
        public int DaysRemaining { get; set; }
        [JsonPropertyName("sessions_remaining")]
        public int SessionsRemaining { get; set; }
        [JsonPropertyName("total_sessions")]
        public int TotalSessions { get; set; }
    }

    // Response training locations
    public class TrainingLocationsResponse
    {
        public int Count { get; set; }
        public List<string> Data { get; set; }
    }
}