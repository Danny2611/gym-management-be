using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Quan hệ với Member
        [BsonElement("member_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }

        // Quan hệ với Package
        [BsonElement("package_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PackageId { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "pending"; // pending, completed, failed, cancelled

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; } // ví dụ: "momo", "credit_card", "cash"

        [BsonElement("transactionId")]
        public string TransactionId { get; set; }

        // Thông tin chi tiết thanh toán (JSON nested)
        [BsonElement("paymentInfo")]
        public PaymentInfo PaymentInfo { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class PaymentInfo
    {
        [BsonElement("requestId")]
        public string RequestId { get; set; }

        [BsonElement("payUrl")]
        public string PayUrl { get; set; }

        [BsonElement("orderId")]
        public string OrderId { get; set; }
    }
}
