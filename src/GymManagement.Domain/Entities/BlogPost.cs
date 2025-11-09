using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class BlogPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("slug")]
        public string Slug { get; set; } // URL thân thiện

        [BsonElement("excerpt")]
        public string Excerpt { get; set; } // tóm tắt bài viết

        [BsonElement("content")]
        public string Content { get; set; } // nội dung HTML

        [BsonElement("coverImage")]
        public string CoverImage { get; set; }

        [BsonElement("publishDate")]
        public DateTime PublishDate { get; set; }

        [BsonElement("readTime")]
        public int ReadTime { get; set; } // thời gian đọc bài (phút)

        [BsonElement("author")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }

        [BsonElement("category")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [BsonElement("featured")]
        public bool Featured { get; set; } = false;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
