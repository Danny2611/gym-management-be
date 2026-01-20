
using GymManagement.Application.Interfaces.Repositories.Public;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Public
{
    public class BlogCategoryRepository : IBlogCategoryRepository
    {
        private readonly IMongoCollection<BlogCategory> _blogCategories;

        public BlogCategoryRepository(MongoDbContext context)
        {
            _blogCategories = context.BlogCategories;
        }

        public async Task<List<BlogCategory>> GetAllCategoriesAsync()
        {
            return await _blogCategories
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<BlogCategory> GetCategoryBySlugAsync(string slug)
        {
            return await _blogCategories
                .Find(c => c.Slug == slug)
                .FirstOrDefaultAsync();
        }

        public async Task<BlogCategory> GetByIdAsync(string id)
        {
            return await _blogCategories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}