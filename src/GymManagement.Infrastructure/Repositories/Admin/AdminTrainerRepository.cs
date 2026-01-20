using MongoDB.Driver;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Application.DTOs.Admin.Trainer;
using MongoDB.Bson;



namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminTrainerRepository : IAdminTrainerRepository
    {
        private readonly IMongoCollection<Trainer> _trainers;

        public AdminTrainerRepository(MongoDbContext context)
        {
            _trainers = context.Trainers;
        }

        public async Task<(List<Trainer> trainers, int totalCount)> GetAllAsync(TrainerQueryOptions options)
        {
            var filterBuilder = Builders<Trainer>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(options.Status))
                filter &= filterBuilder.Eq(t => t.Status, options.Status);

            if (!string.IsNullOrEmpty(options.Specialization))
                filter &= filterBuilder.Eq(t => t.Specialization, options.Specialization);

            if (options.Experience.HasValue)
                filter &= filterBuilder.Gte(t => t.Experience, options.Experience.Value);

            if (!string.IsNullOrEmpty(options.Search))
            {
                var searchFilter = filterBuilder.Or(
                    filterBuilder.Regex("name", new MongoDB.Bson.BsonRegularExpression(options.Search, "i")),
                    filterBuilder.Regex("email", new MongoDB.Bson.BsonRegularExpression(options.Search, "i")),
                    filterBuilder.Regex("specialization", new MongoDB.Bson.BsonRegularExpression(options.Search, "i"))
                );
                filter &= searchFilter;
            }

            var totalCount = await _trainers.CountDocumentsAsync(filter);

            var sortBuilder = Builders<Trainer>.Sort;
            var sort = !string.IsNullOrEmpty(options.SortBy)
                ? (options.SortOrder?.ToLower() == "desc" ? sortBuilder.Descending(options.SortBy) : sortBuilder.Ascending(options.SortBy))
                : sortBuilder.Descending("created_at");

            var trainers = await _trainers.Find(filter).Sort(sort)
                .Skip((options.Page - 1) * options.Limit).Limit(options.Limit).ToListAsync();

            return (trainers, (int)totalCount);
        }

        public async Task<Trainer> GetByIdAsync(string trainerId) =>
            await _trainers.Find(t => t.Id == trainerId).FirstOrDefaultAsync();

        public async Task<Trainer> GetByEmailAsync(string email) =>
            await _trainers.Find(t => t.Email == email).FirstOrDefaultAsync();

        public async Task<Trainer> CreateAsync(Trainer trainer)
        {
            await _trainers.InsertOneAsync(trainer);
            return trainer;
        }

        public async Task<Trainer> UpdateAsync(string trainerId, Trainer trainer)
        {
            trainer.UpdatedAt = DateTime.UtcNow;
            await _trainers.ReplaceOneAsync(t => t.Id == trainerId, trainer);
            return trainer;
        }

        public async Task<Trainer> UpdateScheduleAsync(string trainerId, List<TrainerSchedule> schedule)
        {
            var update = Builders<Trainer>.Update
                .Set(t => t.Schedule, schedule)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            return await _trainers.FindOneAndUpdateAsync(
                t => t.Id == trainerId,
                update,
                new FindOneAndUpdateOptions<Trainer> { ReturnDocument = ReturnDocument.After });
        }

        public async Task<bool> SoftDeleteAsync(string trainerId)
        {
            var now = DateTime.UtcNow;

            var filter = Builders<Trainer>.Filter.And(
                Builders<Trainer>.Filter.Eq(t => t.Id, trainerId),
                Builders<Trainer>.Filter.Or(
                    Builders<Trainer>.Filter.Eq(t => t.DeletedAt, null),
                    Builders<Trainer>.Filter.Exists(t => t.DeletedAt, false)
                )
            );

            var update = Builders<Trainer>.Update
                .Set(t => t.DeletedAt, now)
                .Set(t => t.Status, "inactive");

            var result = await _trainers.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }


        public async Task<Trainer> ToggleStatusAsync(string trainerId)
        {
            var trainer = await GetByIdAsync(trainerId);
            if (trainer == null) throw new KeyNotFoundException("Không tìm thấy huấn luyện viên");

            var newStatus = trainer.Status == "active" ? "inactive" : "active";
            var update = Builders<Trainer>.Update
                .Set(t => t.Status, newStatus)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            return await _trainers.FindOneAndUpdateAsync(
                t => t.Id == trainerId, update,
                new FindOneAndUpdateOptions<Trainer> { ReturnDocument = ReturnDocument.After });
        }

        public async Task<TrainerStatsDto> GetStatsAsync()
        {
            var total = await _trainers.CountDocumentsAsync(FilterDefinition<Trainer>.Empty);
            var active = await _trainers.CountDocumentsAsync(t => t.Status == "active");
            var inactive = await _trainers.CountDocumentsAsync(t => t.Status == "inactive");

            var specPipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument { { "_id", "$specialization" }, { "count", new BsonDocument("$sum", 1) } })
            };
            var specCounts = await _trainers.Aggregate<BsonDocument>(specPipeline).ToListAsync();
            var bySpec = specCounts.ToDictionary(
                s => s.GetValue("_id", "unspecified").ToString(),
                s => s.GetValue("count", 0).ToInt32());

            var novice = await _trainers.CountDocumentsAsync(t => t.Experience >= 0 && t.Experience <= 2);
            var intermediate = await _trainers.CountDocumentsAsync(t => t.Experience >= 3 && t.Experience <= 5);
            var experienced = await _trainers.CountDocumentsAsync(t => t.Experience >= 6 && t.Experience <= 10);
            var expert = await _trainers.CountDocumentsAsync(t => t.Experience >= 11);

            return new TrainerStatsDto
            {
                Total = (int)total,
                Active = (int)active,
                Inactive = (int)inactive,
                BySpecialization = bySpec,
                ExperienceRanges = new ExperienceRangesDto
                {
                    Novice = (int)novice,
                    Intermediate = (int)intermediate,
                    Experienced = (int)experienced,
                    Expert = (int)expert
                }
            };
        }
    }
}

