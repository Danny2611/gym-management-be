
using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;
namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminPackageRepository : IAdminPackageRepository
    {
        private readonly IMongoCollection<Package> _packages;
        private readonly IMongoCollection<PackageDetail> _packageDetails;

        public AdminPackageRepository(MongoDbContext context)
        {
            _packages = context.Packages;
            _packageDetails = context.PackageDetails;
        }

        public async Task<(List<Package> packages, int totalCount)> GetAllAsync(
            PackageQueryOptions options)
        {
            var filterBuilder = Builders<Package>.Filter;
            var filter = filterBuilder.Eq(p => p.DeletedAt, null);

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filter &= filterBuilder.Eq(p => p.Status, options.Status);
            }

            // Category filter
            if (!string.IsNullOrEmpty(options.Category))
            {
                filter &= filterBuilder.Eq(p => p.Category, options.Category);
            }

            // Popular filter
            if (options.Popular.HasValue)
            {
                filter &= filterBuilder.Eq(p => p.Popular, options.Popular.Value);
            }

            // Search filter
            if (!string.IsNullOrEmpty(options.Search))
            {
                filter &= filterBuilder.Regex("name", new MongoDB.Bson.BsonRegularExpression(options.Search, "i"));
            }

            // Get total count
            var totalCount = await _packages.CountDocumentsAsync(filter);

            // Sorting
            var sortBuilder = Builders<Package>.Sort;
            SortDefinition<Package> sort;

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

            // Get packages with pagination
            var packages = await _packages
                .Find(filter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (packages, (int)totalCount);
        }

        public async Task<Package> GetByIdAsync(string packageId)
        {
            return await _packages
                .Find(p => p.Id == packageId && p.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<PackageDetail> GetDetailByPackageIdAsync(string packageId)
        {
            return await _packageDetails
                .Find(pd => pd.PackageId == packageId && pd.DeletedAt == null && pd.Status == "active")
                .FirstOrDefaultAsync();
        }

        public async Task<Package> CreateAsync(Package package)
        {
            await _packages.InsertOneAsync(package);
            return package;
        }

        public async Task<PackageDetail> CreateDetailAsync(PackageDetail packageDetail)
        {
            await _packageDetails.InsertOneAsync(packageDetail);
            return packageDetail;
        }

        public async Task<Package> UpdateAsync(string packageId, Package package)
        {
            package.UpdatedAt = DateTime.UtcNow;
            await _packages.ReplaceOneAsync(
                p => p.Id == packageId && p.DeletedAt == null,
                package);
            return package;
        }

        public async Task<PackageDetail> UpdateDetailAsync(string packageId, PackageDetail packageDetail)
        {
            packageDetail.UpdatedAt = DateTime.UtcNow;

            var options = new FindOneAndUpdateOptions<PackageDetail>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var update = Builders<PackageDetail>.Update
                .Set(pd => pd.Schedule, packageDetail.Schedule)
                .Set(pd => pd.TrainingAreas, packageDetail.TrainingAreas)
                .Set(pd => pd.AdditionalServices, packageDetail.AdditionalServices)
                .Set(pd => pd.UpdatedAt, DateTime.UtcNow)
                .SetOnInsert(pd => pd.PackageId, packageId)
                .SetOnInsert(pd => pd.Status, "active")
                .SetOnInsert(pd => pd.CreatedAt, DateTime.UtcNow);

            return await _packageDetails.FindOneAndUpdateAsync(
                pd => pd.PackageId == packageId && pd.DeletedAt == null,
                update,
                options);
        }

        public async Task<bool> SoftDeleteAsync(string packageId)
        {
            var now = DateTime.UtcNow;

            // Soft delete package
            var packageUpdate = Builders<Package>.Update
                .Set(p => p.DeletedAt, now)
                .Set(p => p.Status, "inactive");

            await _packages.UpdateOneAsync(
                p => p.Id == packageId && p.DeletedAt == null,
                packageUpdate);

            // Soft delete package details
            var detailUpdate = Builders<PackageDetail>.Update
                .Set(pd => pd.DeletedAt, now)
                .Set(pd => pd.Status, "inactive");

            await _packageDetails.UpdateOneAsync(
                pd => pd.PackageId == packageId && pd.DeletedAt == null,
                detailUpdate);

            return true;
        }

        public async Task<Package> ToggleStatusAsync(string packageId)
        {
            var package = await GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói dịch vụ");
            }

            var newStatus = package.Status == "active" ? "inactive" : "active";

            var update = Builders<Package>.Update
                .Set(p => p.Status, newStatus)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            return await _packages.FindOneAndUpdateAsync(
                p => p.Id == packageId,
                update,
                new FindOneAndUpdateOptions<Package>
                {
                    ReturnDocument = ReturnDocument.After
                });
        }

        public async Task<PackageStatsDto> GetStatsAsync()
        {
            var filter = Builders<Package>.Filter.Eq(p => p.DeletedAt, null);

            var total = await _packages.CountDocumentsAsync(filter);
            var active = await _packages.CountDocumentsAsync(
                Builders<Package>.Filter.And(filter, Builders<Package>.Filter.Eq(p => p.Status, "active")));
            var inactive = await _packages.CountDocumentsAsync(
                Builders<Package>.Filter.And(filter, Builders<Package>.Filter.Eq(p => p.Status, "inactive")));
            var popular = await _packages.CountDocumentsAsync(
                Builders<Package>.Filter.And(filter, Builders<Package>.Filter.Eq(p => p.Popular, true)));
            var withTrainingSessions = await _packages.CountDocumentsAsync(
                Builders<Package>.Filter.And(filter, Builders<Package>.Filter.Gt(p => p.TrainingSessions, 0)));

            // Get counts by category
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("deleted_at", BsonNull.Value)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$category" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var categoryCounts = await _packages.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var byCategory = new Dictionary<string, int>();
            foreach (var category in categoryCounts)
            {
                var categoryName = category.GetValue("_id", "basic").AsString;
                var count = category.GetValue("count", 0).ToInt32();
                byCategory[categoryName] = count;
            }

            return new PackageStatsDto
            {
                Total = (int)total,
                Active = (int)active,
                Inactive = (int)inactive,
                ByCategory = byCategory,
                Popular = (int)popular,
                WithTrainingSessions = (int)withTrainingSessions
            };
        }
    }
}