using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Trainer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("image")]
        public string Image { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("bio")]
        public string Bio { get; set; }

        [BsonElement("specialization")]
        public string Specialization { get; set; }

        [BsonElement("experience")]
        public int Experience { get; set; } // Số năm kinh nghiệm

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("schedule")]
        public List<TrainerSchedule> Schedule { get; set; } = new List<TrainerSchedule>();

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class TrainerSchedule
    {
        [BsonElement("dayOfWeek")]
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ...

        [BsonElement("available")]
        public bool Available { get; set; } = true;

        [BsonElement("workingHours")]
        public List<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
    }

    public class WorkingHour
    {
        [BsonElement("start")]
        public string Start { get; set; } // "08:00"

        [BsonElement("end")]
        public string End { get; set; }   // "12:00"

        [BsonElement("available")]
        public bool Available { get; set; } = true;
    }
}
