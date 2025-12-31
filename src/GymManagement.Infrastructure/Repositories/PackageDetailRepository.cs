using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories
{
    public class PackageDetailRepository : IPackageDetailRepository
    {
        private readonly IMongoCollection<PackageDetail> _packageDetails;

        public PackageDetailRepository(MongoDbContext context)
        {
            _packageDetails = context.PackageDetails;
        }

        public async Task<List<PackageDetail>> GetByPackageIdsAsync(List<string> packageIds)
        {
            return await _packageDetails
                .Find(pd => packageIds.Contains(pd.PackageId) && pd.Status == "active")
                .ToListAsync();
        }

        public async Task<PackageDetail> GetByIdAsync(string id)
        {
            return await _packageDetails
                .Find(pd => pd.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PackageDetail> GetByPackageIdAsync(string packageId)
        {
            return await _packageDetails
                .Find(pd => pd.PackageId == packageId)
                .FirstOrDefaultAsync();
        }
    }
}
