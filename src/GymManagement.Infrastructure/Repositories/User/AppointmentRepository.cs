using MongoDB.Driver;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentRepository(MongoDbContext context)
        {
            _appointments = context.Appointments;
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await _appointments.InsertOneAsync(appointment);
            return appointment;
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await _appointments.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Appointment>> GetByMemberIdAsync(string memberId)
        {
            return await _appointments
                .Find(a => a.MemberId == memberId)
                .SortBy(a => a.Date)
                .ThenBy(a => a.Time.Start)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByMemberIdWithFiltersAsync(
            string memberId,
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? searchTerm)
        {
            var fb = Builders<Appointment>.Filter;
            var filter = fb.Eq(a => a.MemberId, memberId);

            if (startDate.HasValue)
                filter &= fb.Gte(a => a.Date, startDate.Value);

            if (endDate.HasValue)
                filter &= fb.Lte(a => a.Date, endDate.Value);

            if (!string.IsNullOrEmpty(status))
                filter &= fb.Eq(a => a.Status, status);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var regex = new MongoDB.Bson.BsonRegularExpression(searchTerm, "i");

                filter &= fb.Or(
                    fb.Regex(a => a.Notes, regex),
                    fb.Regex(a => a.Location, regex)
                );
            }

            return await _appointments
                .Find(filter)
                .SortBy(a => a.Date)
                .ThenBy(a => a.Time.Start)
                .ToListAsync();
        }


        public async Task<List<Appointment>> GetConflictingAppointmentsAsync(
            string trainerId,
            DateTime date,
            string startTime,
            string endTime)
        {
            var filterBuilder = Builders<Appointment>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(a => a.TrainerId, trainerId),
                filterBuilder.Eq(a => a.Date, date),
                filterBuilder.In(a => a.Status, new[] { "confirmed", "pending" }),
                filterBuilder.Or(
                    filterBuilder.And(
                        filterBuilder.Lt("time.start", endTime),
                        filterBuilder.Gte("time.start", startTime)
                    ),
                    filterBuilder.And(
                        filterBuilder.Gt("time.end", startTime),
                        filterBuilder.Lte("time.end", endTime)
                    ),
                    filterBuilder.And(
                        filterBuilder.Lte("time.start", startTime),
                        filterBuilder.Gte("time.end", endTime)
                    )
                )
            );

            return await _appointments.Find(filter).ToListAsync();
        }

        public async Task UpdateAsync(string id, Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.UtcNow;
            await _appointments.ReplaceOneAsync(a => a.Id == id, appointment);
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync(
            string memberId,
            DateTime fromDate,
            DateTime toDate)
        {
            var filter = Builders<Appointment>.Filter.And(
                Builders<Appointment>.Filter.Eq(a => a.MemberId, memberId),
                Builders<Appointment>.Filter.Gte(a => a.Date, fromDate),
                Builders<Appointment>.Filter.Lt(a => a.Date, toDate),
                Builders<Appointment>.Filter.In(a => a.Status, new[] { "confirmed", "pending" })
            );

            return await _appointments
                .Find(filter)
                .SortBy(a => a.Date)
                .ThenBy(a => a.Time.Start)
                .ToListAsync();
        }

        public async Task UpdateMissedAppointmentsAsync(DateTime yesterday)
        {
            var endOfYesterday = yesterday.AddHours(23).AddMinutes(59).AddSeconds(59);

            var filter = Builders<Appointment>.Filter.And(
                Builders<Appointment>.Filter.Gte(a => a.Date, yesterday),
                Builders<Appointment>.Filter.Lte(a => a.Date, endOfYesterday),
                Builders<Appointment>.Filter.Eq(a => a.Status, "confirmed")
            );

            var update = Builders<Appointment>.Update.Set(a => a.Status, "missed");

            await _appointments.UpdateManyAsync(filter, update);
        }
    }


}