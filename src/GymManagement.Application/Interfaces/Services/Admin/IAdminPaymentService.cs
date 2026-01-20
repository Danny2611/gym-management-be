using GymManagement.Application.DTOs.Admin;

namespace GymManagement.Application.Interfaces.Services.Admin
{
    public interface IAdminPaymentService
    {
        Task<PaymentListResponseDto> GetAllPaymentsAsync(PaymentQueryOptions options);
        Task<PaymentResponseDto> GetPaymentByIdAsync(string id);
        Task<PaymentResponseDto> UpdatePaymentStatusAsync(string id, string status, string? transactionId);
        Task<PaymentListResponseDto> GetPaymentsByMemberIdAsync(string memberId, PaymentQueryOptions options);
        Task<PaymentStatisticsDto> GetPaymentStatisticsAsync();
    }
}