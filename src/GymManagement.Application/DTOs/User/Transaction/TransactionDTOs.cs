using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Transaction
{
    // Filter request
    public class TransactionFilterDto
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
    }

    // Request get transaction by ID
    public class GetTransactionRequest
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }
    }

    // Response transaction item (for list)
    public class TransactionItemDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("packageName")]
        public string PackageName { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("paymentInfo")]
        public object PaymentInfo { get; set; }
    }

    // Response transaction detail
    public class TransactionDetailDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("member_id")]
        public MemberBasicDto MemberId { get; set; }

        [JsonPropertyName("package_id")]
        public PackageDetailDto PackageId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("packageName")]
        public string PackageName { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("paymentInfo")]
        public object PaymentInfo { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MemberBasicDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }
    }

    public class PackageDetailDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    // Recent transaction DTO
    public class RecentTransactionDto
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("packageName")]
        public string? PackageName { get; set; }
    }
}