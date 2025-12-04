using MongoDB.Driver;
using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;

namespace GymManagement.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MongoDbContext _context;

        public RoleRepository(MongoDbContext context)
        {
             _context = context;
        }

        public async Task<Role?> GetByIdAsync(string id)
        {
            var filter = Builders<Role>.Filter.Eq(x => x.Id, id);
            return await _context.Roles.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            var filter = Builders<Role>.Filter.Eq(x => x.Name, name);
            return await _context.Roles.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.Find(_ => true).ToListAsync();
        }
    }
}