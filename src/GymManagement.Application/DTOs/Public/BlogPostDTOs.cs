using System.Text.Json.Serialization;

namespace GymManagement.Application.DTOs.Public
{
    // Author DTO
    public class BlogAuthorDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    // Category DTO
    public class BlogCategoryDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("post_count")]
        public int PostCount { get; set; }
    }

    // Blog Post Response DTO
    public class BlogPostResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("excerpt")]
        public string Excerpt { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("cover_image")]
        public string CoverImage { get; set; }

        [JsonPropertyName("publish_date")]
        public string PublishDate { get; set; }

        [JsonPropertyName("read_time")]
        public int ReadTime { get; set; }

        [JsonPropertyName("author")]
        public BlogAuthorDto Author { get; set; }

        [JsonPropertyName("category")]
        public BlogCategoryDto Category { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("featured")]
        public bool Featured { get; set; }
    }

    // Paginated Response DTO
    public class BlogPostListResponseDto
    {
        [JsonPropertyName("posts")]
        public List<BlogPostResponseDto> Posts { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }

    // Category Response DTO
    public class BlogCategoryResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("post_count")]
        public int PostCount { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}