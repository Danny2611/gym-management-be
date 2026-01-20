using GymManagement.Application.DTOs.Admin;
using GymManagement.Domain.Entities;

namespace GymManagement.Application.Interfaces.Repositories.Admin
{
    public interface IAdminPaymentRepository
    {
        Task<(List<Payment> payments, int totalCount)> GetAllAsync(PaymentQueryOptions options);
        Task<Payment?> GetByIdAsync(string id);
        Task<Payment?> UpdateStatusAsync(string id, string status, string? transactionId);
        Task<(List<Payment> payments, int totalCount)> GetByMemberIdAsync(
            string memberId,
            PaymentQueryOptions options);
        Task<PaymentStatisticsDto> GetStatisticsAsync();
    }
}