using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class WorkoutSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("member_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MemberId { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("timeStart")]
        public DateTime TimeStart { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; } // phút

        [BsonElement("muscle_groups")]
        public List<string> MuscleGroups { get; set; } = new List<string>();

        [BsonElement("exercises")]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "pending"; // pending, completed, cancelled

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("sets")]
        public int Sets { get; set; }

        [BsonElement("reps")]
        public int Reps { get; set; }

        [BsonElement("weight")]
        public double Weight { get; set; } // kg
    }
}
