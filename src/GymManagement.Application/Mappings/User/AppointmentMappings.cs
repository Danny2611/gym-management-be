using GymManagement.Application.DTOs.User.Appointment;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Mappings.User
{
    public static class AppointmentMappings
    {
        public static AppointmentDto ToDto(
            this Appointment appointment,
            TrainerBasicDto trainer,
            string packageName
        )
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                Trainer = trainer,
                Date = appointment.Date.ToString("yyyy-MM-dd"),
                Time = $"{appointment.Time.Start} - {appointment.Time.End}",
                Location = appointment.Location,
                Status = appointment.Status,
                Notes = appointment.Notes,
                PackageName = packageName
            };
        }

        // private static string FormatTime(AppointmentTime time)
        // {
        //     if (time == null) return null;

        //     return $"{time.Start} - {time.End}";
        // }
    }
}
