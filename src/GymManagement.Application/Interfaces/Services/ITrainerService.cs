using GymManagement.Domain.Entities;

public interface ITrainerService
{
    Task<List<Trainer>> GetTrainersAsync();
    Task<Trainer?> GetTrainerByIdAsync(Guid id);
}
