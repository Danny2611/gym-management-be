using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminAppointmentRepository
    {
        Task<(List<Appointment> appointments, int totalCount)> GetAllAsync(AppointmentQueryOptions options);
        Task<Appointment?> GetByIdAsync(string id);
        Task<Appointment?> UpdateStatusAsync(string id, string status);
        Task<AppointmentStatsDto> GetStatsAsync();
    }
}