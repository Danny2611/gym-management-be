using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly IMongoCollection<Promotion> _promotions;

        public PromotionRepository(MongoDbContext context)
        {
            _promotions = context.Promotions;
        }

        public async Task<List<Promotion>> GetAllActivePromotionsAsync()
        {
            var currentDate = DateTime.UtcNow;

            var filter = Builders<Promotion>.Filter.And(
                Builders<Promotion>.Filter.Eq(p => p.Status, "active"),
                Builders<Promotion>.Filter.Lte(p => p.StartDate, currentDate),
                Builders<Promotion>.Filter.Gte(p => p.EndDate, currentDate)
            );

            return await _promotions.Find(filter).ToListAsync();
        }

        public async Task<List<Promotion>> GetAllPromotionsAsync()
        {
            return await _promotions.Find(_ => true).ToListAsync();
        }

        public async Task<Promotion?> GetByIdAsync(string id)
        {
            return await _promotions.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            await _promotions.InsertOneAsync(promotion);
            return promotion;
        }

        public async Task UpdateAsync(string id, Promotion promotion)
        {
            promotion.UpdatedAt = DateTime.UtcNow;
            await _promotions.ReplaceOneAsync(p => p.Id == id, promotion);
        }

        public async Task DeleteAsync(string id)
        {
            await _promotions.DeleteOneAsync(p => p.Id == id);
        }

        public async Task<Promotion?> GetActivePromotionByPackageIdAsync(string packageId, DateTime now)
        {
            return await _promotions.Find(p =>
                p.ApplicablePackages.Contains(packageId) &&
                p.Status == "active" &&
                p.StartDate <= now &&
                p.EndDate >= now
            ).FirstOrDefaultAsync();
        }
    }
}