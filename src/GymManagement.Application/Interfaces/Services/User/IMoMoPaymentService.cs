

using GymManagement.Application.DTOs.User.Payment;

namespace GymManagement.Application.Interfaces.Services.User
{
    public interface IMoMoPaymentService
    {
        Task<MoMoPaymentResponse> CreatePaymentRequestAsync(string packageId, string memberId, long amount, string orderInfo);
        bool VerifyCallback(MoMoIpnCallbackDto callbackData);
        MoMoExtraData DecodeExtraData(string extraData);
    }
}