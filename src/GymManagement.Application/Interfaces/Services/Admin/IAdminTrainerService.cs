using GymManagement.Application.DTOs.Admin.Trainer;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminTrainerService
    {
        Task<TrainerListResponseDto> GetAllTrainersAsync(TrainerQueryOptions options);
        Task<TrainerResponseDto> GetTrainerByIdAsync(string trainerId);
        Task<TrainerResponseDto> CreateTrainerAsync(CreateTrainerDto dto);
        Task<TrainerResponseDto> UpdateTrainerAsync(string trainerId, UpdateTrainerDto dto);
        Task<TrainerResponseDto> UpdateTrainerScheduleAsync(string trainerId, List<TrainerScheduleDto> schedule);
        Task<bool> DeleteTrainerAsync(string trainerId);
        Task<TrainerResponseDto> ToggleTrainerStatusAsync(string trainerId);
        Task<TrainerAvailabilityDto> GetTrainerAvailabilityAsync(string trainerId, DateTime startDate, DateTime endDate);
        Task<TrainerStatsDto> GetTrainerStatsAsync();
    }
}