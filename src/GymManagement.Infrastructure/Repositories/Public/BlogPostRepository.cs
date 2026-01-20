
using GymManagement.Application.Interfaces.Repositories.Public;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Public
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly IMongoCollection<BlogPost> _blogPosts;

        public BlogPostRepository(MongoDbContext context)
        {
            _blogPosts = context.BlogPosts;
        }

        public async Task<List<BlogPost>> GetLatestPostsAsync(int limit)
        {
            return await _blogPosts
                .Find(_ => true)
                .SortByDescending(p => p.PublishDate)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetFeaturedPostsAsync(int limit)
        {
            return await _blogPosts
                .Find(p => p.Featured == true)
                .SortByDescending(p => p.PublishDate)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<(List<BlogPost> posts, int total)> GetAllPostsAsync(int page, int pageSize)
        {
            var total = await _blogPosts.CountDocumentsAsync(_ => true);

            var posts = await _blogPosts
                .Find(_ => true)
                .SortByDescending(p => p.PublishDate)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (posts, (int)total);
        }

        public async Task<(List<BlogPost> posts, int total)> GetPostsByCategoryAsync(
            string categoryId, int page, int pageSize)
        {
            var filter = Builders<BlogPost>.Filter.Eq(p => p.CategoryId, categoryId);
            var total = await _blogPosts.CountDocumentsAsync(filter);

            var posts = await _blogPosts
                .Find(filter)
                .SortByDescending(p => p.PublishDate)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (posts, (int)total);
        }

        public async Task<(List<BlogPost> posts, int total)> GetPostsByTagAsync(
            string tag, int page, int pageSize)
        {
            var filter = Builders<BlogPost>.Filter.AnyEq(p => p.Tags, tag);
            var total = await _blogPosts.CountDocumentsAsync(filter);

            var posts = await _blogPosts
                .Find(filter)
                .SortByDescending(p => p.PublishDate)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (posts, (int)total);
        }

        public async Task<BlogPost> GetPostBySlugAsync(string slug)
        {
            return await _blogPosts
                .Find(p => p.Slug == slug)
                .FirstOrDefaultAsync();
        }
    }
}