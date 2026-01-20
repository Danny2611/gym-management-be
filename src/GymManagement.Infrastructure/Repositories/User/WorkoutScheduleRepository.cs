using MongoDB.Driver;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class WorkoutScheduleRepository : IWorkoutScheduleRepository
    {
        private readonly IMongoCollection<WorkoutSchedule> _workoutSchedules;

        public WorkoutScheduleRepository(MongoDbContext context)
        {
            _workoutSchedules = context.WorkoutSchedules;
        }

        public async Task<WorkoutSchedule> CreateAsync(WorkoutSchedule schedule)
        {
            await _workoutSchedules.InsertOneAsync(schedule);
            return schedule;
        }

        public async Task<List<WorkoutSchedule>> GetByMemberIdAsync(
            string memberId,
            DateTime? startDate,
            DateTime? endDate,
            string status)
        {
            var filterBuilder = Builders<WorkoutSchedule>.Filter;
            var filter = filterBuilder.Eq(w => w.MemberId, memberId);

            if (!string.IsNullOrEmpty(status))
            {
                filter &= filterBuilder.Eq(w => w.Status, status);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                if (startDate.HasValue)
                {
                    filter &= filterBuilder.Gte(w => w.Date, startDate.Value);
                }
                if (endDate.HasValue)
                {
                    filter &= filterBuilder.Lte(w => w.Date, endDate.Value);
                }
            }

            return await _workoutSchedules
                .Find(filter)
                .SortBy(w => w.Date)
                .ThenBy(w => w.TimeStart)
                .ToListAsync();
        }

        public async Task<WorkoutSchedule> GetByIdAsync(string scheduleId)
        {
            return await _workoutSchedules
                .Find(w => w.Id == scheduleId)
                .FirstOrDefaultAsync();
        }

        public async Task<WorkoutSchedule> UpdateStatusAsync(string scheduleId, string status)
        {
            var update = Builders<WorkoutSchedule>.Update
                .Set(w => w.Status, status)
                .Set(w => w.UpdatedAt, DateTime.UtcNow);

            return await _workoutSchedules.FindOneAndUpdateAsync(
                w => w.Id == scheduleId,
                update,
                new FindOneAndUpdateOptions<WorkoutSchedule>
                {
                    ReturnDocument = ReturnDocument.After
                });
        }

        public async Task<List<WorkoutSchedule>> GetCompletedByDateRangeAsync(
            string memberId,
            DateTime startDate,
            DateTime endDate)
        {
            var filter = Builders<WorkoutSchedule>.Filter.And(
                Builders<WorkoutSchedule>.Filter.Eq(w => w.MemberId, memberId),
                Builders<WorkoutSchedule>.Filter.Gte(w => w.Date, startDate),
                Builders<WorkoutSchedule>.Filter.Lte(w => w.Date, endDate),
                Builders<WorkoutSchedule>.Filter.Eq(w => w.Status, "completed")
            );

            return await _workoutSchedules.Find(filter).ToListAsync();
        }

        public async Task<List<WorkoutSchedule>> GetMissedByDateRangeAsync(
            string memberId,
            DateTime startDate,
            DateTime endDate)
        {
            var filter = Builders<WorkoutSchedule>.Filter.And(
                Builders<WorkoutSchedule>.Filter.Eq(w => w.MemberId, memberId),
                Builders<WorkoutSchedule>.Filter.Gte(w => w.Date, startDate),
                Builders<WorkoutSchedule>.Filter.Lte(w => w.Date, endDate),
                Builders<WorkoutSchedule>.Filter.Eq(w => w.Status, "missed")
            );

            return await _workoutSchedules.Find(filter).ToListAsync();
        }

        public async Task<List<WorkoutSchedule>> GetByDateRangeAsync(
            string memberId,
            DateTime startDate,
            DateTime endDate)
        {
            var filter = Builders<WorkoutSchedule>.Filter.And(
                Builders<WorkoutSchedule>.Filter.Eq(w => w.MemberId, memberId),
                Builders<WorkoutSchedule>.Filter.Gte(w => w.Date, startDate),
                Builders<WorkoutSchedule>.Filter.Lte(w => w.Date, endDate)
            );

            return await _workoutSchedules
                .Find(filter)
                .SortBy(w => w.Date)
                .ToListAsync();
        }

        public async Task<List<WorkoutSchedule>> GetUpcomingAsync(
            string memberId,
            DateTime startDate,
            DateTime endDate)
        {
            var filter = Builders<WorkoutSchedule>.Filter.And(
                Builders<WorkoutSchedule>.Filter.Eq(w => w.MemberId, memberId),
                Builders<WorkoutSchedule>.Filter.Gte(w => w.Date, startDate),
                Builders<WorkoutSchedule>.Filter.Lt(w => w.Date, endDate),
                Builders<WorkoutSchedule>.Filter.Eq(w => w.Status, "upcoming")
            );

            return await _workoutSchedules
                .Find(filter)
                .SortBy(w => w.Date)
                .ThenBy(w => w.TimeStart)
                .ToListAsync();
        }

        public async Task<long> UpdateMissedSchedulesAsync(DateTime now)
        {
            var filter = Builders<WorkoutSchedule>.Filter.And(
                Builders<WorkoutSchedule>.Filter.Eq(w => w.Status, "upcoming"),
                Builders<WorkoutSchedule>.Filter.Where(w =>
                    w.TimeStart.AddMinutes(w.Duration) < now
                )
            );

            var update = Builders<WorkoutSchedule>.Update
                .Set(w => w.Status, "missed")
                .Set(w => w.UpdatedAt, DateTime.UtcNow);

            var result = await _workoutSchedules.UpdateManyAsync(filter, update);
            return result.ModifiedCount;
        }
    }


}


