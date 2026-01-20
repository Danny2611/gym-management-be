using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.User
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(string id);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<Payment> CreateAsync(Payment payment);
        Task UpdateAsync(string id, Payment payment);
        Task<List<Payment>> GetByMemberIdAsync(string memberId);
        Task<List<Payment>> GetByMemberIdWithFiltersAsync(string memberId, string? status, string? paymentMethod, DateTime? startDate, DateTime? endDate);
        Task<List<Payment>> GetRecentSuccessfulByMemberIdAsync(string memberId, int limit);
    }


}