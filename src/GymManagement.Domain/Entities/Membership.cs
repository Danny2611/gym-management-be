using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Membership
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

        // Quan hệ với Payment
        [BsonElement("payment_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PaymentId { get; set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("auto_renew")]
        public bool AutoRenew { get; set; } = false;

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, expired, cancelled

        [BsonElement("available_sessions")]
        public int AvailableSessions { get; set; } = 0;

        [BsonElement("used_sessions")]
        public int UsedSessions { get; set; } = 0;

        [BsonElement("last_sessions_reset")]
        public DateTime LastSessionsReset { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
