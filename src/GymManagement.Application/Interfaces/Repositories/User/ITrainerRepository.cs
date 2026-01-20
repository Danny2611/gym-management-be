using GymManagement.Domain.Entities;


namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface ITrainerRepository
    {
        Task<List<Trainer>> GetActiveAsync();
        Task<Trainer?> GetByIdAsync(string id);
        Task UpdateAsync(string id, Trainer trainer);
        Task<List<Trainer>> GetAllAsync();
    }

}
