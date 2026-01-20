using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly IMongoCollection<Membership> _memberships;

        public MembershipRepository(MongoDbContext context)
        {
            _memberships = context.Memberships;
        }

        public async Task<Membership?> GetByIdAsync(string id)
        {
            return await _memberships.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Membership?> GetByMemberAndPackageAsync(string memberId, string packageId, string status)
        {
            return await _memberships.Find(m =>
                m.MemberId == memberId &&
                m.PackageId == packageId &&
                m.Status == status
            ).FirstOrDefaultAsync();
        }

        public async Task<List<Membership>> GetByMemberIdAsync(string memberId)
        {
            return await _memberships.Find(m => m.MemberId == memberId).ToListAsync();
        }

        public async Task<List<Membership>> GetAllAsync()
        {
            return await _memberships.Find(_ => true).ToListAsync();
        }

        public async Task<Membership> CreateAsync(Membership membership)
        {
            await _memberships.InsertOneAsync(membership);
            return membership;
        }

        public async Task UpdateAsync(string id, Membership membership)
        {
            membership.UpdatedAt = DateTime.UtcNow;
            await _memberships.ReplaceOneAsync(m => m.Id == id, membership);
        }

        public async Task DeleteManyAsync(string memberId, string packageId, List<string> statuses)
        {
            var filter = Builders<Membership>.Filter.And(
                Builders<Membership>.Filter.Eq(m => m.MemberId, memberId),
                Builders<Membership>.Filter.Eq(m => m.PackageId, packageId),
                Builders<Membership>.Filter.In(m => m.Status, statuses)
            );
            await _memberships.DeleteManyAsync(filter);
        }
    }
}
