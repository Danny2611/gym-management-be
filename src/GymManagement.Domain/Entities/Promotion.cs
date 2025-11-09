using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Promotion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("discount")]
        public double Discount { get; set; } // Phần trăm giảm giá

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("applicable_packages")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> ApplicablePackages { get; set; } = new List<string>();

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, inactive

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
