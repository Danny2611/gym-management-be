using GymManagement.Application.Interfaces.Repositories.User;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Repositories.User
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMongoCollection<Payment> _payments;

        public PaymentRepository(MongoDbContext context)
        {
            _payments = context.Payments;
        }

        public async Task<Payment?> GetByIdAsync(string id)
        {
            return await _payments.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _payments.Find(p => p.TransactionId == transactionId).FirstOrDefaultAsync();
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            await _payments.InsertOneAsync(payment);
            return payment;
        }

        public async Task UpdateAsync(string id, Payment payment)
        {
            var update = Builders<Payment>.Update
                .Set(x => x.Status, payment.Status)
                .Set(x => x.PaymentMethod, payment.PaymentMethod)
                .Set(x => x.PaymentInfo, payment.PaymentInfo)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            if (payment.Promotion != null)
                update = update.Set(x => x.Promotion, payment.Promotion);

            await _payments.UpdateOneAsync(
                p => p.Id == id,
                update
            );
        }


        public async Task<List<Payment>> GetByMemberIdAsync(string memberId)
        {
            return await _payments.Find(p => p.MemberId == memberId).ToListAsync();
        }

        public async Task<List<Payment>> GetByMemberIdWithFiltersAsync(string memberId, string? status, string? paymentMethod, DateTime? startDate, DateTime? endDate)
        {
            var filterBuilder = Builders<Payment>.Filter;
            var filter = filterBuilder.Eq(p => p.MemberId, memberId);

            if (!string.IsNullOrEmpty(status))
                filter &= filterBuilder.Eq(p => p.Status, status);

            if (!string.IsNullOrEmpty(paymentMethod))
                filter &= filterBuilder.Eq(p => p.PaymentMethod, paymentMethod);

            if (startDate.HasValue)
                filter &= filterBuilder.Gte(p => p.CreatedAt, startDate.Value);

            if (endDate.HasValue)
                filter &= filterBuilder.Lte(p => p.CreatedAt, endDate.Value);

            return await _payments
                .Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetRecentSuccessfulByMemberIdAsync(string memberId, int limit)
        {
            var filter = Builders<Payment>.Filter.And(
                Builders<Payment>.Filter.Eq(p => p.MemberId, memberId),
                Builders<Payment>.Filter.Eq(p => p.Status, "completed")
            );

            return await _payments
                .Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .Limit(limit)
                .ToListAsync();

        }
    }
}
