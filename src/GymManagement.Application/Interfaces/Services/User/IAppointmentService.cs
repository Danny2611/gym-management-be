using GymManagement.Application.DTOs.User.Appointment;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(string memberId, CreateAppointmentRequest request);
        Task<List<AppointmentDto>> GetAllMemberAppointmentsAsync(string memberId, string? status, DateTime? startDate, DateTime? endDate, string? searchTerm);
        Task<AppointmentDto> GetAppointmentByIdAsync(string appointmentId);
        Task<List<ScheduleDto>> GetMemberScheduleAsync(string memberId, string? status, DateTime? startDate, DateTime? endDate, string? timeSlot, string? searchTerm);
        Task<AppointmentDto> CancelAppointmentAsync(string appointmentId, string memberId);
        Task<AppointmentDto> RescheduleAppointmentAsync(string appointmentId, string memberId, RescheduleAppointmentRequest request);
        //    Task<AvailabilityResponse> CheckTrainerAvailabilityAsync(CheckAvailabilityRequest request);
        Task<List<UpcomingAppointmentDto>> GetUpcomingAppointmentsAsync(string memberId);
        Task UpdateMissedAppointmentsAsync();
    }
}