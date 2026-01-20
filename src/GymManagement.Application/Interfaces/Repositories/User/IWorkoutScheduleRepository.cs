

using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IWorkoutScheduleRepository
    {
        Task<WorkoutSchedule> CreateAsync(WorkoutSchedule schedule);
        Task<List<WorkoutSchedule>> GetByMemberIdAsync(string memberId, DateTime? startDate, DateTime? endDate, string status);
        Task<WorkoutSchedule> GetByIdAsync(string scheduleId);
        Task<WorkoutSchedule> UpdateStatusAsync(string scheduleId, string status);
        Task<List<WorkoutSchedule>> GetCompletedByDateRangeAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<List<WorkoutSchedule>> GetMissedByDateRangeAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<List<WorkoutSchedule>> GetByDateRangeAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<List<WorkoutSchedule>> GetUpcomingAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<long> UpdateMissedSchedulesAsync(DateTime now);
    }


}