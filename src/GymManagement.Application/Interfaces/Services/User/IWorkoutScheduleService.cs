using GymManagement.Application.DTOs.User.Workout;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Services.User
{
    public interface IWorkoutScheduleService
    {
        Task<WorkoutSchedule> CreateWorkoutScheduleAsync(CreateWorkoutScheduleDto dto, string memberId);
        Task<List<WorkoutSchedule>> GetMemberWorkoutSchedulesAsync(string memberId, DateTime? startDate, DateTime? endDate, string status);
        Task<WorkoutSchedule> GetWorkoutScheduleByIdAsync(string scheduleId, string memberId);
        Task<WorkoutSchedule> UpdateWorkoutScheduleStatusAsync(string scheduleId, string status, string memberId);
        Task<List<WeeklyWorkoutDto>> GetWeeklyWorkoutStatsAsync(string memberId, DateTime? startDate);
        Task<MonthComparisonDto> GetMonthComparisonStatsAsync(string memberId);
        Task<List<RecentWorkoutLogDto>> GetLast7DaysWorkoutsAsync(string memberId);
        Task<List<WorkoutScheduleNextWeekDto>> GetUpcomingWorkoutsAsync(string memberId);
    }
}