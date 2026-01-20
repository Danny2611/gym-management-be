using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace GymManagement.Application.DTOs.Admin
{
    // Query Options
    public class PaymentQueryOptions
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; } // pending, completed, failed, cancelled
        public string? PaymentMethod { get; set; } // qr, credit, napas, undefined
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } // asc, desc
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    // Response DTOs
    public class PaymentResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member")]
        public PaymentMemberDto Member { get; set; }

        [JsonPropertyName("package")]
        public PaymentPackageDto Package { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("paymentInfo")]
        public object? PaymentInfo { get; set; }

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class PaymentMemberDto
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

    public class PaymentPackageDto
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
    public class UpdatePaymentStatusDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }
    }

    // List Response DTO
    public class PaymentListResponseDto
    {
        [JsonPropertyName("payments")]
        public List<PaymentResponseDto> Payments { get; set; }

        [JsonPropertyName("total_payments")]
        public int TotalPayments { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
    }

    // Statistics Response DTO
    public class PaymentStatisticsDto
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("completed")]
        public int Completed { get; set; }

        [JsonPropertyName("failed")]
        public int Failed { get; set; }

        [JsonPropertyName("cancelled")]
        public int Cancelled { get; set; }

        [JsonPropertyName("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("completed_revenue")]
        public decimal CompletedRevenue { get; set; }

        [JsonPropertyName("payment_methods")]
        public PaymentMethodStatsDto PaymentMethods { get; set; }

        [JsonPropertyName("monthly_revenue")]
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; }
    }

    public class PaymentMethodStatsDto
    {
        [JsonPropertyName("qr")]
        public int Qr { get; set; }

        [JsonPropertyName("credit")]
        public int Credit { get; set; }

        [JsonPropertyName("napas")]
        public int Napas { get; set; }

        [JsonPropertyName("undefined")]
        public int Undefined { get; set; }
    }

    public class MonthlyRevenueDto
    {
        [JsonPropertyName("month")]
        public string Month { get; set; }

        [JsonPropertyName("revenue")]
        public decimal Revenue { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}