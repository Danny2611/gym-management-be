using MongoDB.Driver;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.Interfaces.Repositories.User;


namespace GymManagement.Infrastructure.Repositories.Admin
{

    public class AdminMemberRepository : IAdminMemberRepository
    {
        private readonly IMongoCollection<Member> _members;
        private readonly IRoleRepository _roleRepository;

        public AdminMemberRepository(MongoDbContext context, IRoleRepository roleRepository)
        {
            _members = context.Members;
            _roleRepository = roleRepository;
        }

        public async Task<(List<Member> members, int totalCount)> GetAllAsync(MemberQueryOptions options)
        {
            var filterBuilder = Builders<Member>.Filter;
            var filter = filterBuilder.Empty;

            // Search filter
            if (!string.IsNullOrEmpty(options.Search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex("name", new MongoDB.Bson.BsonRegularExpression(options.Search, "i")),
                    filterBuilder.Regex("email", new MongoDB.Bson.BsonRegularExpression(options.Search, "i")),
                    filterBuilder.Regex("phone", new MongoDB.Bson.BsonRegularExpression(options.Search, "i"))
                );
                filter &= searchFilter;
            }

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filter &= filterBuilder.Eq(m => m.Status, options.Status);
            }

            // Get total count
            var totalCount = await _members.CountDocumentsAsync(filter);

            // Sorting
            var sortBuilder = Builders<Member>.Sort;
            SortDefinition<Member> sort;

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

            // Get members with pagination
            var members = await _members
                .Find(filter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (members, (int)totalCount);
        }

        public async Task<Member> GetByIdAsync(string memberId)
        {
            return await _members.Find(m => m.Id == memberId).FirstOrDefaultAsync();
        }

        public async Task<Member> GetByEmailAsync(string email)
        {
            return await _members.Find(m => m.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Member> CreateAsync(Member member)
        {
            await _members.InsertOneAsync(member);
            return member;
        }

        public async Task<Member> UpdateAsync(string memberId, Member member)
        {
            member.UpdatedAt = DateTime.UtcNow;
            await _members.ReplaceOneAsync(m => m.Id == memberId, member);
            return member;
        }

        public async Task<Member> UpdateStatusAsync(string memberId, string status)
        {
            var update = Builders<Member>.Update
                .Set(m => m.Status, status)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);

            var member = await _members.FindOneAndUpdateAsync(
                m => m.Id == memberId,
                update,
                new FindOneAndUpdateOptions<Member>
                {
                    ReturnDocument = ReturnDocument.After
                });

            return member;
        }

        public async Task<bool> DeleteAsync(string memberId)
        {
            var result = await _members.DeleteOneAsync(m => m.Id == memberId);
            return result.DeletedCount > 0;
        }

        public async Task<MemberStatsDto> GetStatsAsync()
        {
            var total = await _members.CountDocumentsAsync(FilterDefinition<Member>.Empty);
            var active = await _members.CountDocumentsAsync(m => m.Status == "active");
            var inactive = await _members.CountDocumentsAsync(m => m.Status == "inactive");
            var pending = await _members.CountDocumentsAsync(m => m.Status == "pending");
            var banned = await _members.CountDocumentsAsync(m => m.Status == "banned");
            var verified = await _members.CountDocumentsAsync(m => m.IsVerified == true);
            var unverified = await _members.CountDocumentsAsync(m => m.IsVerified == false);

            return new MemberStatsDto
            {
                Total = (int)total,
                Active = (int)active,
                Inactive = (int)inactive,
                Pending = (int)pending,
                Banned = (int)banned,
                Verified = (int)verified,
                Unverified = (int)unverified
            };
        }
    }
}