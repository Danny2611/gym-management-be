using MongoDB.Driver;
using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly IMongoCollection<Trainer> _trainers;

        public TrainerRepository(MongoDbContext context)
        {
            _trainers = context.Trainers;
        }

        public async Task<List<Trainer>> GetActiveAsync()
        {
            return await _trainers
                 .Find(p => p.Status == "active")
                 .ToListAsync();
        }

        public async Task<Trainer?> GetByIdAsync(string id)
        {
            return await _trainers.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(string id, Trainer trainer)
        {
            trainer.UpdatedAt = DateTime.UtcNow;
            await _trainers.ReplaceOneAsync(t => t.Id == id, trainer);
        }

        public async Task<List<Trainer>> GetAllAsync()
        {
            return await _trainers.Find(_ => true).ToListAsync();
        }


    }


}