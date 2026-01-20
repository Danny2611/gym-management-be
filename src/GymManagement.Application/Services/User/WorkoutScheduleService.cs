using GymManagement.Application.DTOs.User.Workout;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;


namespace GymManagement.Application.Services.User
{

    public class WorkoutScheduleService : IWorkoutScheduleService
    {
        private readonly IWorkoutScheduleRepository _repository;

        public WorkoutScheduleService(IWorkoutScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<WorkoutSchedule> CreateWorkoutScheduleAsync(CreateWorkoutScheduleDto dto, string memberId)
        {
            var schedule = new WorkoutSchedule
            {
                MemberId = memberId,
                Date = dto.Date,
                TimeStart = dto.TimeStart,
                Duration = dto.Duration,
                MuscleGroups = dto.MuscleGroups ?? new List<string>(),
                Exercises = dto.Exercises?.Select(e => new Exercise
                {
                    Name = e.Name,
                    Sets = e.Sets,
                    Reps = e.Reps,
                    Weight = e.Weight
                }).ToList() ?? new List<Exercise>(),
                Location = dto.Location,
                Notes = dto.Notes,
                Status = "upcoming",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(schedule);
        }

        public async Task<List<WorkoutSchedule>> GetMemberWorkoutSchedulesAsync(
            string memberId,
            DateTime? startDate,
            DateTime? endDate,
            string status)
        {
            return await _repository.GetByMemberIdAsync(memberId, startDate, endDate, status);
        }

        public async Task<WorkoutSchedule> GetWorkoutScheduleByIdAsync(string scheduleId, string memberId)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);

            if (schedule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin lịch tập");
            }

            if (schedule.MemberId != memberId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xem lịch tập này");
            }

            return schedule;
        }

        public async Task<WorkoutSchedule> UpdateWorkoutScheduleStatusAsync(
            string scheduleId,
            string status,
            string memberId)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);

            if (schedule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin lịch tập");
            }

            if (schedule.MemberId != memberId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật lịch tập này");
            }

            return await _repository.UpdateStatusAsync(scheduleId, status);
        }

        public async Task<List<WeeklyWorkoutDto>> GetWeeklyWorkoutStatsAsync(string memberId, DateTime? startDate)
        {
            var today = startDate ?? DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);

            var schedules = await _repository.GetCompletedByDateRangeAsync(memberId, weekStart, weekEnd);

            var weeklyMap = new Dictionary<string, WeeklyWorkoutDto>
            {
                ["T2"] = new WeeklyWorkoutDto { Name = "T2", Sessions = 0, Duration = 0, Target = 1 },
                ["T3"] = new WeeklyWorkoutDto { Name = "T3", Sessions = 0, Duration = 0, Target = 1 },
                ["T4"] = new WeeklyWorkoutDto { Name = "T4", Sessions = 0, Duration = 0, Target = 1 },
                ["T5"] = new WeeklyWorkoutDto { Name = "T5", Sessions = 0, Duration = 0, Target = 1 },
                ["T6"] = new WeeklyWorkoutDto { Name = "T6", Sessions = 0, Duration = 0, Target = 1 },
                ["T7"] = new WeeklyWorkoutDto { Name = "T7", Sessions = 0, Duration = 0, Target = 1 },
                ["CN"] = new WeeklyWorkoutDto { Name = "CN", Sessions = 0, Duration = 0, Target = 1 }
            };

            foreach (var schedule in schedules)
            {
                var dayLabel = GetDayOfWeekLabel(schedule.Date);
                if (weeklyMap.ContainsKey(dayLabel))
                {
                    weeklyMap[dayLabel].Sessions += 1;
                    weeklyMap[dayLabel].Duration += schedule.Duration;
                }
            }

            return weeklyMap.Values.ToList();
        }

        public async Task<MonthComparisonDto> GetMonthComparisonStatsAsync(string memberId)
        {
            var currentMonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddDays(-1);

            var currentCompleted = await _repository.GetCompletedByDateRangeAsync(
                memberId, currentMonthStart, currentMonthEnd);
            var currentMissed = await _repository.GetMissedByDateRangeAsync(
                memberId, currentMonthStart, currentMonthEnd);

            var previousCompleted = await _repository.GetCompletedByDateRangeAsync(
                memberId, previousMonthStart, previousMonthEnd);
            var previousMissed = await _repository.GetMissedByDateRangeAsync(
                memberId, previousMonthStart, previousMonthEnd);

            return new MonthComparisonDto
            {
                Current = CalculateMonthlyStats(currentCompleted, currentMissed),
                Previous = CalculateMonthlyStats(previousCompleted, previousMissed)
            };
        }

        public async Task<List<RecentWorkoutLogDto>> GetLast7DaysWorkoutsAsync(string memberId)
        {
            var today = DateTime.Today.AddDays(1).AddSeconds(-1);
            var sevenDaysAgo = DateTime.Today.AddDays(-6);

            var workouts = await _repository.GetByDateRangeAsync(memberId, sevenDaysAgo, today);

            return workouts.Select(w => new RecentWorkoutLogDto
            {
                CreatedAt = w.CreatedAt,
                MuscleGroups = w.MuscleGroups,
                Duration = w.Duration,
                Status = w.Status
            }).ToList();
        }

        public async Task<List<WorkoutScheduleNextWeekDto>> GetUpcomingWorkoutsAsync(string memberId)
        {
            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            var upcomingWorkouts = await _repository.GetUpcomingAsync(memberId, today, nextWeek);

            return upcomingWorkouts.Select(w => new WorkoutScheduleNextWeekDto
            {
                Date = w.Date,
                TimeStart = w.TimeStart,
                TimeEnd = w.TimeStart.AddMinutes(w.Duration),
                Location = w.Location,
                Status = w.Status
            }).ToList();
        }

        private string GetDayOfWeekLabel(DateTime date)
        {
            var dayLabels = new[] { "CN", "T2", "T3", "T4", "T5", "T6", "T7" };
            return dayLabels[(int)date.DayOfWeek];
        }

        private MonthlyStatsDto CalculateMonthlyStats(
            List<WorkoutSchedule> completed,
            List<WorkoutSchedule> missed)
        {
            var totalSessions = completed.Count;
            var totalDuration = completed.Sum(s => s.Duration);
            var avgSessionLength = totalSessions > 0 ? totalDuration / totalSessions : 0;

            var totalScheduled = completed.Count + missed.Count;
            var completionRate = totalScheduled > 0
                ? (int)Math.Round((double)completed.Count / totalScheduled * 100)
                : 0;

            return new MonthlyStatsDto
            {
                TotalSessions = totalSessions,
                TotalDuration = totalDuration,
                CompletionRate = completionRate,
                AvgSessionLength = avgSessionLength
            };
        }
    }
}