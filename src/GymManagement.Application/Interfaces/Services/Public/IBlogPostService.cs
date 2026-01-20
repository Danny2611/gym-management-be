using GymManagement.Application.DTOs.Public;

namespace GymManagement.Application.Interfaces.Services.Public
{
    public interface IBlogPostService
    {
        Task<List<BlogPostResponseDto>> GetLatestPostsAsync(int limit);
        Task<List<BlogPostResponseDto>> GetFeaturedPostsAsync(int limit);
        Task<BlogPostListResponseDto> GetAllPostsAsync(int page, int pageSize);
        Task<BlogPostListResponseDto> GetPostsByCategoryAsync(string categorySlug, int page, int pageSize);
        Task<BlogPostListResponseDto> GetPostsByTagAsync(string tag, int page, int pageSize);
        Task<BlogPostResponseDto> GetPostBySlugAsync(string slug);
    }


}