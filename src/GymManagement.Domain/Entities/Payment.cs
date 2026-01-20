// GymManagement.Domain/Entities/Payment.cs
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

        [BsonElement("member_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }

        [BsonElement("package_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PackageId { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "pending";

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; }

        [BsonElement("transactionId")]
        public string TransactionId { get; set; }

        // ✅ QUAN TRỌNG NHẤT
        [BsonElement("paymentInfo")]
        public BsonDocument PaymentInfo { get; set; } = new();

        [BsonElement("promotion")]
        public AppliedPromotion? Promotion { get; set; }


        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class AppliedPromotion
    {
        [BsonElement("promotion_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PromotionId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("discount")]
        public decimal Discount { get; set; }

        [BsonElement("original_price")]
        public decimal OriginalPrice { get; set; }

        [BsonElement("discounted_price")]
        public decimal DiscountedPrice { get; set; }
    }
}