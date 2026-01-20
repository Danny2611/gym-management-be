using GymManagement.Application.DTOs.User.Workout;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.User
{
    public static class WorkoutScheduleMappings
    {
        public static WorkoutScheduleResponseDto ToDto(this WorkoutSchedule schedule)
        {
            return new WorkoutScheduleResponseDto
            {
                Id = schedule.Id,
                MemberId = schedule.MemberId,
                Date = schedule.Date,
                TimeStart = schedule.TimeStart,
                Duration = schedule.Duration,
                MuscleGroups = schedule.MuscleGroups,
                Exercises = schedule.Exercises?.Select(e => new ExerciseDto
                {

                    Name = e.Name,
                    Sets = e.Sets,
                    Reps = e.Reps,
                    Weight = e.Weight
                }).ToList() ?? new List<ExerciseDto>(),
                Location = schedule.Location,
                Status = schedule.Status,
                Notes = schedule.Notes,
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt
            };
        }

        public static WorkoutSchedule ToEntity(this CreateWorkoutScheduleDto dto, string memberId)
        {
            return new WorkoutSchedule
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
        }

        public static RecentWorkoutLogDto ToRecentLogDto(this WorkoutSchedule schedule)
        {
            return new RecentWorkoutLogDto
            {
                CreatedAt = schedule.CreatedAt,
                MuscleGroups = schedule.MuscleGroups,
                Duration = schedule.Duration,
                Status = schedule.Status
            };
        }

        public static WorkoutScheduleNextWeekDto ToNextWeekDto(this WorkoutSchedule schedule)
        {
            var endTime = schedule.TimeStart.AddMinutes(schedule.Duration);

            return new WorkoutScheduleNextWeekDto
            {
                Date = schedule.Date,
                TimeStart = schedule.TimeStart,
                TimeEnd = endTime,
                Location = schedule.Location,
                Status = schedule.Status
            };
        }
    }
}