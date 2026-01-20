
using GymManagement.Application.DTOs.User.Payment;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface IPaymentService
    {
        Task<PaymentCreatedResponse> CreateMoMoPaymentAsync(string memberId, CreatePaymentRequest request);
        Task ProcessMoMoIpnCallbackAsync(MoMoIpnCallbackDto callbackData);
        Task<PaymentStatusResponse> GetPaymentStatusAsync(string memberId, string paymentId, string? userRole);
        Task<Domain.Entities.Payment> GetPaymentByIdAsync(string memberId, string paymentId, string? userRole);
    }
}