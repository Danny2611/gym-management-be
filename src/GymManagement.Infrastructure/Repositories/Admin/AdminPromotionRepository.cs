using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminPromotionRepository : IAdminPromotionRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Promotion> _promotions;

        public AdminPromotionRepository(MongoDbContext context)
        {
            _context = context;
            _promotions = _context.Promotions;
        }

        public async Task<(List<Promotion> promotions, int totalCount)> GetAllAsync(
            PromotionQueryOptions options)
        {
            var filterBuilder = Builders<Promotion>.Filter;
            var filters = new List<FilterDefinition<Promotion>>();

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filters.Add(filterBuilder.Eq(p => p.Status, options.Status));
            }

            // Search filter (by name or description)
            if (!string.IsNullOrEmpty(options.Search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex(p => p.Name, new BsonRegularExpression(options.Search, "i")),
                    filterBuilder.Regex(p => p.Description, new BsonRegularExpression(options.Search, "i"))
                );
                filters.Add(searchFilter);
            }

            var finalFilter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            // Get total count
            var totalCount = (int)await _promotions.CountDocumentsAsync(finalFilter);

            // Build sort
            var sortBuilder = Builders<Promotion>.Sort;
            SortDefinition<Promotion> sort;

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                sort = options.SortOrder?.ToLower() == "desc"
                    ? sortBuilder.Descending(options.SortBy)
                    : sortBuilder.Ascending(options.SortBy);
            }
            else
            {
                sort = sortBuilder.Descending(p => p.CreatedAt);
            }

            // Get promotions with pagination
            var promotions = await _promotions
                .Find(finalFilter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (promotions, totalCount);
        }

        public async Task<Promotion?> GetByIdAsync(string id)
        {
            return await _promotions
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            await _promotions.InsertOneAsync(promotion);
            return promotion;
        }

        public async Task<Promotion?> UpdateAsync(string id, UpdatePromotionDto updateData)
        {
            var updateBuilder = Builders<Promotion>.Update;
            var updates = new List<UpdateDefinition<Promotion>>();

            if (!string.IsNullOrEmpty(updateData.Name))
            {
                updates.Add(updateBuilder.Set(p => p.Name, updateData.Name.Trim()));
            }

            if (updateData.Description != null)
            {
                updates.Add(updateBuilder.Set(p => p.Description, updateData.Description.Trim()));
            }

            if (updateData.Discount.HasValue)
            {
                updates.Add(updateBuilder.Set(p => p.Discount, updateData.Discount.Value));
            }

            if (updateData.StartDate.HasValue)
            {
                updates.Add(updateBuilder.Set(p => p.StartDate, updateData.StartDate.Value));
            }

            if (updateData.EndDate.HasValue)
            {
                updates.Add(updateBuilder.Set(p => p.EndDate, updateData.EndDate.Value));
            }

            if (!string.IsNullOrEmpty(updateData.Status))
            {
                updates.Add(updateBuilder.Set(p => p.Status, updateData.Status));
            }

            if (updateData.ApplicablePackages != null && updateData.ApplicablePackages.Any())
            {
                updates.Add(updateBuilder.Set(p => p.ApplicablePackages, updateData.ApplicablePackages));
            }

            updates.Add(updateBuilder.Set(p => p.UpdatedAt, DateTime.UtcNow));

            var combinedUpdate = updateBuilder.Combine(updates);

            var options = new FindOneAndUpdateOptions<Promotion>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _promotions.FindOneAndUpdateAsync(
                p => p.Id == id,
                combinedUpdate,
                options
            );
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _promotions.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Promotion>> GetActivePromotionsForPackageAsync(string packageId)
        {
            var now = DateTime.UtcNow;

            var filter = Builders<Promotion>.Filter.And(
                Builders<Promotion>.Filter.AnyEq(p => p.ApplicablePackages, packageId),
                Builders<Promotion>.Filter.Eq(p => p.Status, "active"),
                Builders<Promotion>.Filter.Lte(p => p.StartDate, now),
                Builders<Promotion>.Filter.Gte(p => p.EndDate, now)
            );

            return await _promotions
                .Find(filter)
                .ToListAsync();
        }

        public async Task<PromotionStatsDto> GetStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var total = (int)await _promotions.CountDocumentsAsync(FilterDefinition<Promotion>.Empty);

            var active = (int)await _promotions.CountDocumentsAsync(p =>
                p.Status == "active" &&
                p.StartDate <= now &&
                p.EndDate >= now
            );

            var inactive = (int)await _promotions.CountDocumentsAsync(p => p.Status == "inactive");

            var expiredThisMonth = (int)await _promotions.CountDocumentsAsync(p =>
                p.EndDate >= startOfMonth &&
                p.EndDate <= endOfMonth &&
                p.EndDate < now
            );

            var upcomingThisMonth = (int)await _promotions.CountDocumentsAsync(p =>
                p.StartDate >= now &&
                p.StartDate <= endOfMonth
            );

            return new PromotionStatsDto
            {
                Total = total,
                Active = active,
                Inactive = inactive,
                ExpiredThisMonth = expiredThisMonth,
                UpcomingThisMonth = upcomingThisMonth
            };
        }

        public async Task UpdatePromotionStatusesAsync()
        {
            var now = DateTime.UtcNow;

            var filter = Builders<Promotion>.Filter.And(
                Builders<Promotion>.Filter.Eq(p => p.Status, "active"),
                Builders<Promotion>.Filter.Lt(p => p.EndDate, now)
            );

            var update = Builders<Promotion>.Update
                .Set(p => p.Status, "inactive")
                .Set(p => p.UpdatedAt, now);

            await _promotions.UpdateManyAsync(filter, update);
        }
    }
}