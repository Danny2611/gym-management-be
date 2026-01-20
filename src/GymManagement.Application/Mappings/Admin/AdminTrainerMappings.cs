using GymManagement.Application.DTOs.Admin.Trainer;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings
{
    public static class TrainerMappings
    {
        public static TrainerResponseDto ToDto(this Trainer trainer)
        {
            return new TrainerResponseDto
            {
                Id = trainer.Id,
                Image = trainer.Image,
                Name = trainer.Name,
                Bio = trainer.Bio,
                Specialization = trainer.Specialization,
                Experience = trainer.Experience,
                Phone = trainer.Phone,
                Email = trainer.Email,
                Schedule = trainer.Schedule?.Select(s => new TrainerScheduleDto
                {
                    DayOfWeek = s.DayOfWeek,
                    Available = s.Available,
                    WorkingHours = s.WorkingHours?.Select(w => new WorkingHourDto
                    {
                        Start = w.Start,
                        End = w.End,
                        Available = w.Available
                    }).ToList() ?? new List<WorkingHourDto>()
                }).ToList() ?? new List<TrainerScheduleDto>(),
                Status = trainer.Status,
                CreatedAt = trainer.CreatedAt,
                UpdatedAt = trainer.UpdatedAt
            };
        }

        public static Trainer ToEntity(this CreateTrainerDto dto)
        {
            var schedule = dto.Schedule ?? Enumerable.Range(0, 7).Select(i => new TrainerScheduleDto
            {
                DayOfWeek = i,
                Available = false,
                WorkingHours = new List<WorkingHourDto>()
            }).ToList();

            return new Trainer
            {
                Image = dto.Image,
                Name = dto.Name,
                Bio = dto.Bio,
                Specialization = dto.Specialization,
                Experience = dto.Experience ?? 0,
                Phone = dto.Phone,
                Email = dto.Email,
                Schedule = schedule.Select(s => new TrainerSchedule
                {
                    DayOfWeek = s.DayOfWeek,
                    Available = s.Available,
                    WorkingHours = s.WorkingHours?.Select(w => new WorkingHour
                    {
                        Start = w.Start,
                        End = w.End,
                        Available = w.Available
                    }).ToList() ?? new List<WorkingHour>()
                }).ToList(),
                Status = dto.Status ?? "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static List<TrainerSchedule> ToScheduleEntities(this List<TrainerScheduleDto> dtos)
        {
            return dtos.Select(s => new TrainerSchedule
            {
                DayOfWeek = s.DayOfWeek,
                Available = s.Available,
                WorkingHours = s.WorkingHours?.Select(w => new WorkingHour
                {
                    Start = w.Start,
                    End = w.End,
                    Available = w.Available
                }).ToList() ?? new List<WorkingHour>()
            }).ToList();
        }
    }
}