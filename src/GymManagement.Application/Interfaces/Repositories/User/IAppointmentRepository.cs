using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment?> GetByIdAsync(string id);
        Task<List<Appointment>> GetByMemberIdAsync(string memberId);
        Task<List<Appointment>> GetByMemberIdWithFiltersAsync(
            string memberId,
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? searchTerm
        );

        Task<List<Appointment>> GetConflictingAppointmentsAsync(string trainerId, DateTime date, string startTime, string endTime);
        Task UpdateAsync(string id, Appointment appointment);
        Task<List<Appointment>> GetUpcomingAppointmentsAsync(string memberId, DateTime fromDate, DateTime toDate);
        Task UpdateMissedAppointmentsAsync(DateTime yesterday);
    }

}