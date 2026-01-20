using GymManagement.Application.DTOs.Admin;
using GymManagement.Application.Interfaces.Repositories.Admin;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.Admin
{
    public class AdminAppointmentRepository : IAdminAppointmentRepository
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Appointment> _appointments;

        public AdminAppointmentRepository(MongoDbContext context)
        {
            _context = context;
            _appointments = _context.Appointments;
        }

        public async Task<(List<Appointment> appointments, int totalCount)> GetAllAsync(
            AppointmentQueryOptions options)
        {
            var filterBuilder = Builders<Appointment>.Filter;
            var filters = new List<FilterDefinition<Appointment>>();

            // Status filter
            if (!string.IsNullOrEmpty(options.Status))
            {
                filters.Add(filterBuilder.Eq(a => a.Status, options.Status));
            }

            // Date range filter
            if (options.StartDate.HasValue && options.EndDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(a => a.Date, options.StartDate.Value));
                filters.Add(filterBuilder.Lte(a => a.Date, options.EndDate.Value));
            }
            else if (options.StartDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(a => a.Date, options.StartDate.Value));
            }
            else if (options.EndDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(a => a.Date, options.EndDate.Value));
            }

            // Member filter
            if (!string.IsNullOrEmpty(options.MemberId))
            {
                filters.Add(filterBuilder.Eq(a => a.MemberId, options.MemberId));
            }

            // Trainer filter
            if (!string.IsNullOrEmpty(options.TrainerId))
            {
                filters.Add(filterBuilder.Eq(a => a.TrainerId, options.TrainerId));
            }

            var finalFilter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            // Get total count
            var totalCount = (int)await _appointments.CountDocumentsAsync(finalFilter);

            // Build sort
            var sortBuilder = Builders<Appointment>.Sort;
            SortDefinition<Appointment> sort;

            if (!string.IsNullOrEmpty(options.SortBy))
            {
                sort = options.SortOrder?.ToLower() == "desc"
                    ? sortBuilder.Descending(options.SortBy)
                    : sortBuilder.Ascending(options.SortBy);
            }
            else
            {
                // Default sort by date and start time
                sort = sortBuilder.Ascending(a => a.Date)
                    .Ascending("time.start");
            }

            // Get appointments with pagination
            var appointments = await _appointments
                .Find(finalFilter)
                .Sort(sort)
                .Skip((options.Page - 1) * options.Limit)
                .Limit(options.Limit)
                .ToListAsync();

            return (appointments, totalCount);
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await _appointments
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Appointment?> UpdateStatusAsync(string id, string status)
        {
            var update = Builders<Appointment>.Update
                .Set(a => a.Status, status)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<Appointment>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _appointments.FindOneAndUpdateAsync(
                a => a.Id == id,
                update,
                options
            );
        }

        public async Task<AppointmentStatsDto> GetStatsAsync()
        {
            var total = (int)await _appointments.CountDocumentsAsync(FilterDefinition<Appointment>.Empty);
            var confirmed = (int)await _appointments.CountDocumentsAsync(a => a.Status == "confirmed");
            var pending = (int)await _appointments.CountDocumentsAsync(a => a.Status == "pending");
            var cancelled = (int)await _appointments.CountDocumentsAsync(a => a.Status == "cancelled");
            var completed = (int)await _appointments.CountDocumentsAsync(a => a.Status == "completed");
            var missed = (int)await _appointments.CountDocumentsAsync(a => a.Status == "missed");

            // Get today's date range
            var today = DateTime.Today;
            var endOfToday = today.AddDays(1).AddMilliseconds(-1);

            // Get week range (7 days from today)
            var endOfWeek = today.AddDays(7).AddMilliseconds(-1);

            // Upcoming appointments today
            var upcomingToday = (int)await _appointments.CountDocumentsAsync(a =>
                a.Date >= today && a.Date <= endOfToday &&
                (a.Status == "confirmed" || a.Status == "pending")
            );

            // Upcoming appointments this week
            var upcomingWeek = (int)await _appointments.CountDocumentsAsync(a =>
                a.Date >= today && a.Date <= endOfWeek &&
                (a.Status == "confirmed" || a.Status == "pending")
            );

            return new AppointmentStatsDto
            {
                Total = total,
                Confirmed = confirmed,
                Pending = pending,
                Cancelled = cancelled,
                Completed = completed,
                Missed = missed,
                UpcomingToday = upcomingToday,
                UpcomingWeek = upcomingWeek
            };
        }
    }
}