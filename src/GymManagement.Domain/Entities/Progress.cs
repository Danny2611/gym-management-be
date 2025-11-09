using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Progress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Quan hệ với Member
        [BsonElement("member_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }

        [BsonElement("weight")]
        public double Weight { get; set; } // kg

        [BsonElement("height")]
        public double Height { get; set; } // cm

        [BsonElement("muscle_mass")]
        public double MuscleMass { get; set; } // %

        [BsonElement("body_fat")]
        public double BodyFat { get; set; } // %

        [BsonElement("bmi")]
        public double BMI { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
