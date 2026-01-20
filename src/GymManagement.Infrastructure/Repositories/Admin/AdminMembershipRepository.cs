
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminMembershipRepository : IAdminMembershipRepository
    {
        private readonly IMongoCollection<Membership> _memberships;

        public AdminMembershipRepository(MongoDbContext context)
        {
            _memberships = context.Memberships;
        }

        public async Task<(List<Membership> memberships, int totalCount)> GetAllAsync(
            MembershipQueryOptions options)
        {
            var filterBuilder = Builders<Membership>.Filter;
            var filter = filterBuilder.Empty;

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filter &= filterBuilder.Eq(m => m.Status, options.Status);
            }

            // Member filter
            if (!string.IsNullOrEmpty(options.MemberId))
            {
                filter &= filterBuilder.Eq(m => m.MemberId, options.MemberId);
            }

            // Package filter
            if (!string.IsNullOrEmpty(options.PackageId))
            {
                filter &= filterBuilder.Eq(m => m.PackageId, options.PackageId);
            }

            // Get total count
            var totalCount = await _memberships.CountDocumentsAsync(filter);

            // Sorting
            var sortBuilder = Builders<Membership>.Sort;
            SortDefinition<Membership> sort;

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                sort = options.SortOrder?.ToLower() == "desc"
                    ? sortBuilder.Descending(options.SortBy)
                    : sortBuilder.Ascending(options.SortBy);
            }
            else
            {
                sort = sortBuilder.Descending("created_at");
            }

            // Get memberships with pagination
            var memberships = await _memberships
                .Find(filter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (memberships, (int)totalCount);
        }

        public async Task<Membership> GetByIdAsync(string membershipId)
        {
            return await _memberships.Find(m => m.Id == membershipId).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(string membershipId)
        {
            var result = await _memberships.DeleteOneAsync(m => m.Id == membershipId);
            return result.DeletedCount > 0;
        }

        public async Task<MembershipStatsDto> GetStatsAsync()
        {
            var total = await _memberships.CountDocumentsAsync(FilterDefinition<Membership>.Empty);
            var active = await _memberships.CountDocumentsAsync(m => m.Status == "active");
            var expired = await _memberships.CountDocumentsAsync(m => m.Status == "expired");
            var pending = await _memberships.CountDocumentsAsync(m => m.Status == "pending");
            var paused = await _memberships.CountDocumentsAsync(m => m.Status == "paused");
            var autoRenew = await _memberships.CountDocumentsAsync(m => m.AutoRenew == true);

            // Calculate sum of available and used sessions using aggregation
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "availableSessions", new BsonDocument("$sum", "$available_sessions") },
                    { "usedSessions", new BsonDocument("$sum", "$used_sessions") }
                })
            };

            var aggregateResult = await _memberships.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            var availableSessions = 0;
            var usedSessions = 0;

            if (aggregateResult != null)
            {
                availableSessions = aggregateResult.GetValue("availableSessions", 0).ToInt32();
                usedSessions = aggregateResult.GetValue("usedSessions", 0).ToInt32();
            }

            return new MembershipStatsDto
            {
                Total = (int)total,
                Active = (int)active,
                Expired = (int)expired,
                Pending = (int)pending,
                Paused = (int)paused,
                AutoRenew = (int)autoRenew,
                AvailableSessions = availableSessions,
                UsedSessions = usedSessions
            };
        }
    }
}