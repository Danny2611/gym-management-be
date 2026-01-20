using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.User.Payment
{
    // Request từ client để tạo thanh toán
    public class CreatePaymentRequest
    {
        public string PackageId { get; set; }
    }

    // Request gửi đến MoMo API
    public class MoMoPaymentRequest
    {
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; } = "Test";
        public string StoreId { get; set; } = "MomoTestStore";
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string Lang { get; set; } = "vi";
        public string RequestType { get; set; }
        public bool AutoCapture { get; set; } = true;
        public string ExtraData { get; set; }
        public string OrderGroupId { get; set; } = "";
        public string Signature { get; set; }
    }

    // Response từ MoMo API
    public class MoMoPaymentResponse
    {
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public string PayUrl { get; set; }
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
    }

    // Response trả về cho client
    public class PaymentCreatedResponse
    {
        public string PaymentId { get; set; }
        public string PayUrl { get; set; }
        public long Amount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal Discount { get; set; }
        public bool PromotionApplied { get; set; }
        public string TransactionId { get; set; }
        public long ExpireTime { get; set; }
    }

    // MoMo Callback Data
    public class MoMoCallbackData
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }
        public string TransId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }
        public string AccessKey { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
    }

    // Extra Data Model
    public class MoMoExtraData
    {
        public string PackageId { get; set; }
        public string MemberId { get; set; }
    }

    // Payment Status Response
    public class PaymentStatusResponse
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; } = default!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; } = default!;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("membership")]
        public MembershipInfoDto? Membership { get; set; }
    }
    public class MembershipInfoDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        // FE yêu cầu key là "package_id"
        [JsonPropertyName("package")]
        public PackageBasicDto Package { get; set; } = default!;
    }
    public class PackageBasicDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;
    }

    public class MoMoIpnCallbackDto
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }          // ORDER_xxx
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }

        public long TransId { get; set; }

        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }
    }

}

