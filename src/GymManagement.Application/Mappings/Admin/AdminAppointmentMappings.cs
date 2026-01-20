using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.Admin
{
    public static class AppointmentMapping
    {
        public static AppointmentResponseDto ToDto(
            this Appointment appointment,
            Member member,
            Trainer trainer)
        {
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                Member = new AppointmentMemberDto
                {
                    Id = member.Id,
                    Name = member.Name
                },
                Trainer = new AppointmentTrainerDto
                {
                    Id = trainer.Id,
                    Name = trainer.Name
                },
                MembershipId = appointment.MembershipId,
                Notes = appointment.Notes,
                Date = appointment.Date,
                Time = new AppointmentTimeDto
                {
                    Start = appointment.Time.Start,
                    End = appointment.Time.End
                },
                Location = appointment.Location,
                Status = appointment.Status,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
        }
    }
}