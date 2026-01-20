using MongoDB.Driver;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly IMongoCollection<Progress> _progress;

        public ProgressRepository(MongoDbContext context)
        {
            _progress = context.Progress;
        }
        public async Task<Progress> CreateAsync(Progress progress)
        {
            await _progress.InsertOneAsync(progress);
            return progress;
        }

        public async Task<Progress> GetLatestAsync(string memberId)
        {
            return await _progress
                .Find(p => p.MemberId == memberId)
                .SortByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Progress> GetInitialAsync(string memberId)
        {
            return await _progress
                .Find(p => p.MemberId == memberId)
                .SortBy(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Progress> GetPreviousMonthAsync(string memberId)
        {
            var records = await _progress
                .Find(p => p.MemberId == memberId)
                .SortByDescending(p => p.CreatedAt)
                .Skip(1)
                .Limit(1)
                .ToListAsync();

            return records.FirstOrDefault();
        }

        public async Task<List<Progress>> GetByDateRangeAsync(
            string memberId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _progress
                .Find(p => p.MemberId == memberId &&
                          p.CreatedAt >= startDate &&
                          p.CreatedAt <= endDate)
                .SortBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Progress>> GetMonthlyMetricsAsync(string memberId)
        {
            // Get the initial record to know when to start
            var initial = await GetInitialAsync(memberId);
            if (initial == null)
                return new List<Progress>();

            var startDate = new DateTime(initial.CreatedAt.Year, initial.CreatedAt.Month, 1);
            var currentDate = DateTime.UtcNow;

            var monthlyMetrics = new List<Progress>();

            // Loop through each month from start to current
            for (var date = startDate; date <= currentDate; date = date.AddMonths(1))
            {
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);

                var latestInMonth = await _progress
                    .Find(p => p.MemberId == memberId &&
                              p.CreatedAt >= firstDayOfMonth &&
                              p.CreatedAt <= lastDayOfMonth)
                    .SortByDescending(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latestInMonth != null)
                {
                    monthlyMetrics.Add(latestInMonth);
                }
            }

            return monthlyMetrics;
        }


    }


}