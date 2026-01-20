using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class PackageDetail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Quan hệ với Package
        [BsonElement("package_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PackageId { get; set; }

        // Lịch tập
        [BsonElement("schedule")]
        public List<string> Schedule { get; set; } = new List<string>();

        // Khu vực tập luyện
        [BsonElement("training_areas")]
        public List<string> TrainingAreas { get; set; } = new List<string>();

        // Dịch vụ bổ sung
        [BsonElement("additional_services")]
        public List<string> AdditionalServices { get; set; } = new List<string>();

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, inactive

        [BsonElement("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
