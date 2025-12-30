using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly IMongoCollection<Membership> _memberships;

        public MembershipRepository(MongoDbContext context)
        {
            _memberships = context.Memberships;
        }

        public async Task<List<Membership>> GetByMemberIdAsync(string memberId)
        {
            return await _memberships
                .Find(m => m.MemberId == memberId)
                .ToListAsync();
        }

        public async Task<List<Membership>> GetActiveMembershipsByMemberIdAsync(string memberId)
        {
            return await _memberships
                .Find(m => m.MemberId == memberId && m.Status == "active")
                .ToListAsync();
        }

        public async Task<Membership> GetByIdAsync(string id)
        {
            return await _memberships
                .Find(m => m.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Membership> CreateAsync(Membership membership)
        {
            await _memberships.InsertOneAsync(membership);
            return membership;
        }

        public async Task UpdateAsync(string id, Membership membership)
        {
            await _memberships.ReplaceOneAsync(m => m.Id == id, membership);
        }

        public async Task DeleteAsync(string id)
        {
            await _memberships.DeleteOneAsync(m => m.Id == id);
        }
    }
}
