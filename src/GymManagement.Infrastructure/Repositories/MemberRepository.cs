using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly MongoDbContext _context;

        public MemberRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _context.Members.Find(_ => true).ToListAsync();
        }

        public async Task<Member> GetByIdAsync(string id)
        {
            return await _context.Members.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Member> CreateAsync(Member member)
        {
            member.CreatedAt = DateTime.Now;
            member.UpdatedAt = DateTime.Now;
            await _context.Members.InsertOneAsync(member);
            return member;
        }

        public async Task<bool> UpdateAsync(string id, Member member)
        {
            member.UpdatedAt = DateTime.Now;
            var result = await _context.Members.ReplaceOneAsync(m => m.Id == id, member);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _context.Members.DeleteOneAsync(m => m.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<Member> GetByEmailAsync(string email)
        {
            return await _context.Members.Find(m => m.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(string email)
        {
            var count = await _context.Members.CountDocumentsAsync(m => m.Email == email);
            return count > 0;
        }
    }
}