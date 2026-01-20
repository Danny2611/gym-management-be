using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminAppointmentService
    {
        Task<AppointmentListResponseDto> GetAllAppointmentsAsync(AppointmentQueryOptions options);
        Task<AppointmentResponseDto> GetAppointmentByIdAsync(string appointmentId);
        Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(string appointmentId, string status);
        Task<AppointmentStatsDto> GetAppointmentStatsAsync();
    }
}