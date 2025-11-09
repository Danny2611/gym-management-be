using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Package
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("max_members")]
        public int MaxMembers { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; } // ngày

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("benefits")]
        public List<string> Benefits { get; set; } = new List<string>();

        [BsonElement("status")]
        public string Status { get; set; } = "inactive"; // active, inactive

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("category")]
        public string Category { get; set; } // basic, premium, etc.

        [BsonElement("popular")]
        public bool Popular { get; set; } = false;

        [BsonElement("training_sessions")]
        public int TrainingSessions { get; set; } // tổng số buổi tập

        [BsonElement("session_duration")]
        public int SessionDuration { get; set; } // phút

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
