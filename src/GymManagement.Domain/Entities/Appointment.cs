using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Quan hệ với Member
        [BsonElement("member_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }

        // Quan hệ với Membership (gói tập đã đăng ký)
        [BsonElement("membership_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MembershipId { get; set; }

        // Quan hệ với Trainer
        [BsonElement("trainer_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TrainerId { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        // Thời gian: start - end
        [BsonElement("time")]
        public AppointmentTime Time { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "pending"; // pending, confirmed, cancelled, completed

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // Embedded document cho thời gian start - end
    [BsonIgnoreExtraElements]
    public class AppointmentTime
    {
        [BsonElement("start")]
        public string Start { get; set; } // "07:00"

        [BsonElement("end")]
        public string End { get; set; }   // "12:00"
    }
}
