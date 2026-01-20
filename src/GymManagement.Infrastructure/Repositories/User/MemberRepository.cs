// GymManagement.Infrastructure/Repositories/MemberRepository.cs
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IMongoCollection<Member> _members;

        public MemberRepository(MongoDbContext context)
        {
            _members = context.Members;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _members.Find(_ => true).ToListAsync();
        }

        public async Task<Member> GetByIdAsync(string id)
        {
            return await _members.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Member> GetByEmailAsync(string email)
        {
            return await _members.Find(m => m.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Member> GetByPhoneAsync(string phone)
        {
            return await _members.Find(m => m.Phone == phone).FirstOrDefaultAsync();
        }

        public async Task<Member> CreateAsync(Member member)
        {
            await _members.InsertOneAsync(member);
            return member;
        }

        public async Task UpdateAsync(string id, Member member)
        {
            await _members.ReplaceOneAsync(m => m.Id == id, member);
        }

        public async Task DeleteAsync(string id)
        {
            await _members.DeleteOneAsync(m => m.Id == id);
        }
    }
}