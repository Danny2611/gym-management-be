using GymManagement.Domain.Entities;

using GymManagement.Application.DTOs.User.Trainer;

namespace GymManagement.Application.Mappings.User
{
    public static class TrainerMapping
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
                Status = trainer.Status,
                Schedule = trainer.Schedule.Select(s => new TrainerScheduleDto
                {
                    DayOfWeek = s.DayOfWeek,
                    Available = s.Available,
                    WorkingHours = s.WorkingHours.Select(w => new WorkingHourDto
                    {
                        Start = w.Start,
                        End = w.End,
                        Available = w.Available
                    }).ToList()
                }).ToList()
            };
        }
    }
}
