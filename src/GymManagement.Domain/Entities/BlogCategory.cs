using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class BlogCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("slug")]
        public string Slug { get; set; } // dùng cho URL, ví dụ "workouts"

        [BsonElement("postCount")]
        public int PostCount { get; set; } = 0; // số bài viết trong category

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
