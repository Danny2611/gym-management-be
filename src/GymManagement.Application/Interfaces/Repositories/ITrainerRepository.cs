using GymManagement.Domain.Entities;

public interface ITrainerRepository
{
    Task<List<Trainer>> GetActiveAsync();
    Task<Trainer?> GetByIdAsync(Guid id);
}
