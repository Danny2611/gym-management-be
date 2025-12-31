using GymManagement.Application.Interfaces.Repositories;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _payments;

        public PaymentRepository(MongoDbContext context)
        {
            _payments = context.Payments;
        }

        public async Task<Payment> GetByIdAsync(string id)
        {
            return await _payments
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetByIdsAsync(List<string> ids)
        {
            return await _payments
                .Find(p => ids.Contains(p.Id))
                .ToListAsync();
        }
    }
}
