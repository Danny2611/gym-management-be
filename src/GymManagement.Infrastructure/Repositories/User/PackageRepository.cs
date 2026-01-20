using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IMongoCollection<Package> _packages;

        public PackageRepository(MongoDbContext context)
        {
            _packages = context.Packages;
        }

        public async Task<Package> GetByIdAsync(string id)
        {
            return await _packages
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Package>> GetByIdsAsync(List<string> ids)
        {
            return await _packages
                .Find(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<List<Package>> GetActiveAsync()
        {
            return await _packages
                .Find(p => p.Status == "active")
                .ToListAsync();
        }

        public async Task<PackageDetail?> GetDetailByPackageIdAsync(string packageId)
        {
            // This method would need access to PackageDetail collection
            // For now, return null - should be implemented when needed
            return null;
        }
    }
}
