using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Public
{
    public interface IBlogPostRepository
    {
        Task<List<BlogPost>> GetLatestPostsAsync(int limit);
        Task<List<BlogPost>> GetFeaturedPostsAsync(int limit);
        Task<(List<BlogPost> posts, int total)> GetAllPostsAsync(int page, int pageSize);
        Task<(List<BlogPost> posts, int total)> GetPostsByCategoryAsync(string categoryId, int page, int pageSize);
        Task<(List<BlogPost> posts, int total)> GetPostsByTagAsync(string tag, int page, int pageSize);
        Task<BlogPost> GetPostBySlugAsync(string slug);
    }
}