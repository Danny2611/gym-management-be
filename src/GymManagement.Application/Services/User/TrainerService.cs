using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;

    public TrainerService(ITrainerRepository trainerRepository)
    {
        _trainerRepository = trainerRepository;
    }

    public async Task<List<Trainer>> GetTrainersAsync()
    {
        return await _trainerRepository.GetActiveAsync();
    }

    public async Task<Trainer?> GetTrainerByIdAsync(string id)
    {
        return await _trainerRepository.GetByIdAsync(id);
    }
}
