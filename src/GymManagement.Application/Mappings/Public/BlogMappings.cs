using GymManagement.Application.DTOs.Public;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.Public
{
    public static class BlogMappingExtensions
    {
        public static BlogPostResponseDto ToDto(
            this BlogPost post,
            Member author = null,
            BlogCategory category = null)
        {
            return new BlogPostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                CoverImage = post.CoverImage,
                PublishDate = post.PublishDate.ToString("O"),
                ReadTime = post.ReadTime,
                Author = author != null ? new BlogAuthorDto
                {
                    Id = author.Id,
                    Name = author.Name,
                    // Bio = author.Bio ?? "",
                    Avatar = author.Avatar ?? ""
                } : null,
                Category = category != null ? new BlogCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    PostCount = category.PostCount
                } : null,
                Tags = post.Tags ?? new List<string>(),
                Featured = post.Featured
            };
        }

        public static BlogCategoryResponseDto ToDto(this BlogCategory category)
        {
            return new BlogCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                PostCount = category.PostCount,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}