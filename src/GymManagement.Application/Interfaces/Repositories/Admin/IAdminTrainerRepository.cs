using GymManagement.Application.DTOs.Admin.Trainer;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminTrainerRepository
    {
        Task<(List<Trainer> trainers, int totalCount)> GetAllAsync(TrainerQueryOptions options);
        Task<Trainer> GetByIdAsync(string trainerId);
        Task<Trainer> GetByEmailAsync(string email);
        Task<Trainer> CreateAsync(Trainer trainer);
        Task<Trainer> UpdateAsync(string trainerId, Trainer trainer);
        Task<Trainer> UpdateScheduleAsync(string trainerId, List<TrainerSchedule> schedule);
        Task<bool> SoftDeleteAsync(string trainerId);
        Task<Trainer> ToggleStatusAsync(string trainerId);
        Task<TrainerStatsDto> GetStatsAsync();
    }
}